namespace Doorlist.Application.User;

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

    /// <summary>
    /// Delete user, save change to DB.
    /// </summary>
    private async Task RollbackUserAsync(Guid userId)
    {
        try
        {
            await _userRepository.DeleteUserAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback user {UserId} after registration failure.", userId);
        }
    }
    
    public async Task<Result<UserRegistrationResponseDto>> RegisterLocalAsync(FullUserRegistrationDto registrationData, CancellationToken cancellationToken)
    {
        // Check for ISO 8601 YYYY-MM-DD
        if (!DateTime.TryParse(registrationData.DateOfBirth, out var dateOfBirth))
        {
            return Result<UserRegistrationResponseDto>.Failure("Invalid date format: requires ISO 8601 (YYYY-MM-DD)", ErrorType.Validation);
        }
        
        // Sign-up user.
        User user = new User(registrationData.FirstName, registrationData.LastName, dateOfBirth);
        string? createdKeycloakSub = null;
        try
        {
            // Attempt to save the user to the DB
            await _userRepository.AddUserAsync(user); 
            var dbResult = await _unitOfWork.SaveChangesAsync();
            if (dbResult == 0) return Result<UserRegistrationResponseDto>.Failure("Database save failed", ErrorType.DependencyFailure);
            
            // Attempt registration, rollback on failure.
            var identityResult = await _identityProvisionerService.CreateUserAsync(registrationData.Email, registrationData.Password, user.Id, cancellationToken);
            if (!identityResult.IsSuccess)
            {
                // Delete user, rollback.
                await RollbackUserAsync(user.Id);
                return Result<UserRegistrationResponseDto>.Failure(identityResult.ErrorMessage ?? "", identityResult.ErrorType);
            }
            
            createdKeycloakSub = identityResult.Value; // Store for possible rollback
            
            // Link login record - Issuer: Keycloak
            user.LinkIdentity(_identityProvisionerService.ProviderName, createdKeycloakSub!, _identityProvisionerService.Issuer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Success! 
            return Result<UserRegistrationResponseDto>.Success(new UserRegistrationResponseDto { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for {Email}", registrationData.Email);
            if (createdKeycloakSub != null)
            {
                await _identityProvisionerService.DeleteUserAsync(createdKeycloakSub, cancellationToken);
            }
            await RollbackUserAsync(user.Id);
            return Result<UserRegistrationResponseDto>.Failure(ex.Message, ErrorType.DependencyFailure);
        }
    }
}