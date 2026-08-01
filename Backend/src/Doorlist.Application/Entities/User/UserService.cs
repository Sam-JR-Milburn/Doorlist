namespace Doorlist.Application.Entities.User;

using Microsoft.Extensions.Logging;

using Domain.Entities;
using Domain.Interfaces.User;
using Domain.Utility;
using DTOs;
using Identity;
using Microsoft.AspNetCore.Mvc;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityProvisionerService _identityProvisionerService;
    private readonly IUserRepository _userRepository;
    private readonly IUserLoginRepository _userLoginRepository;
    
    public UserService(ILogger<UserService> logger, IUnitOfWork unitOfWork, IIdentityProvisionerService iIdentityProvisionerService, IUserRepository userRepository, IUserLoginRepository userLoginRepository)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _identityProvisionerService = iIdentityProvisionerService;
        _userRepository = userRepository;
        _userLoginRepository = userLoginRepository;
    }
    
    public async Task<Result<UserRegistrationResponseDto>> RegisterLocalAsync(FullUserRegistrationDto registrationData, CancellationToken cancellationToken)
    {
        // Check for ISO 8601 YYYY-MM-DD
        if (!DateTime.TryParse(registrationData.DateOfBirth, out var dateOfBirth))
        {
            _logger.LogError("Date of birth {DateOfBirth} is not a valid ISP 8601 date", registrationData.DateOfBirth);
            return Result<UserRegistrationResponseDto>.Failure("Invalid date format: requires ISO 8601 (YYYY-MM-DD)", ErrorType.Validation);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        // Sign-up user.
        string? createdKeycloakSub = null;
        try
        {
            User user = new User(registrationData.FirstName, registrationData.LastName, dateOfBirth);
            
            // Attempt to save the user to the DB
            await _userRepository.AddUserAsync(user); 
            var dbResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (dbResult == 0) return Result<UserRegistrationResponseDto>.Failure("Database issues", ErrorType.DependencyFailure);
            
            // Attempt registration, rollback on failure.
            var identityResult = await _identityProvisionerService.CreateUserAsync(registrationData.Email, registrationData.Password, user.Id, cancellationToken);
            if (!identityResult.IsSuccess)
            {
                return Result<UserRegistrationResponseDto>.Failure(identityResult.ErrorMessage ?? "There was a generic failure in creating the user", identityResult.ErrorType);
            }
            
            createdKeycloakSub = identityResult.Value; // Store for possible rollback
            
            // Link login record - Issuer: Keycloak. There is an implicit rollback here on failure, before the commit. 
            user.LinkIdentity(_identityProvisionerService.ProviderName, createdKeycloakSub!, _identityProvisionerService.Issuer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken); // Commit to the DB
            
            // Success! 
            return Result<UserRegistrationResponseDto>.Success(new UserRegistrationResponseDto { UserId = user.Id, FirstName = user.FirstName, LastName = user.LastName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for {Email}", registrationData.Email);
            if (createdKeycloakSub != null)
            {
                try
                {
                    await _identityProvisionerService.DeleteUserAsync(createdKeycloakSub,
                        CancellationToken.None); // Don't cancel the rollback
                }
                catch (Exception kcEx)
                {
                    _logger.LogCritical(kcEx.Message, "Failed to delete Keycloak user {UserId}", createdKeycloakSub);
                }
            }
            // Transaction drops out of scope, implicit rollback
            return Result<UserRegistrationResponseDto>.Failure(ex.Message, ErrorType.DependencyFailure);
        }
    }
    
    // ----
    
}