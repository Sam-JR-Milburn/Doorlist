namespace Doorlist.Application.User;

using Domain.Utility;
using DTOs;

public interface IUserService
{
    public Task<Result<UserRegistrationResponseDto>> RegisterLocalAsync(FullUserRegistrationDto registrationData);
}