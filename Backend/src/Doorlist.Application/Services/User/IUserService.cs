namespace Doorlist.Application.Services.User;

using Domain.Utility;
using DTO.Requests;
using DTO.Responses;

public interface IUserService
{
    public Task<Result<UserRegistrationResponseDto>> RegisterLocalAsync(FullUserRegistrationDto registrationData, CancellationToken cancellationToken);
}