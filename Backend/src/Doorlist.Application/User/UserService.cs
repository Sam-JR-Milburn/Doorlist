namespace Doorlist.Application.User;

using Domain.Entities;
using Domain.Interfaces.User;
using Domain.Utility;
using DTOs;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    ILogger<UserService> _logger;
    IUserRepository _userRepository;
    IUserLoginRepository _userLoginRepository;
    
    public UserService(ILogger<UserService> logger,  IUserRepository userRepository, IUserLoginRepository userLoginRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userLoginRepository = userLoginRepository;
    }

    public async Task<Result<UserRegistrationResponseDto>> RegisterLocalAsync(FullUserRegistrationDto registrationData)
    {
        // Check for ISO 8601 YYYY-MM-DD
        if (!DateTime.TryParse(registrationData.DateOfBirth, out var dateOfBirth))
        {
            return Result<UserRegistrationResponseDto>.Failure("Invalid date format: requires ISO 8601 (YYYY-MM-DD)", ErrorType.Validation);
        }
        
        // Check that this isn't a duplicate signup.
        if (await _userLoginRepository.CheckUserExistsByEmailAsync(registrationData.Email))
        {
            return Result<UserRegistrationResponseDto>.Failure("Email already registered", ErrorType.Conflict);
        }
        
        // Actually register.
        User user = new User(new Guid(), registrationData.FirstName, registrationData.LastName, dateOfBirth);
        
        
        
        
        
        
        throw new NotImplementedException();
    }
}