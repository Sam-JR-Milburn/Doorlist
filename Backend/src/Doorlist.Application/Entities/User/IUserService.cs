namespace Doorlist.Application.Entities.User;

using Domain.Utility;
using DTOs;

public interface IUserService
{
    public Task<Result<UserRegistrationResponseDto>> RegisterLocalAsync(FullUserRegistrationDto registrationData, CancellationToken cancellationToken);
}