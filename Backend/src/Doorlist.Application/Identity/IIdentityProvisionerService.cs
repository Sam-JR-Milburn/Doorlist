namespace Doorlist.Application.Identity;    

using Doorlist.Domain.Utility;

public interface IIdentityProvisionerService
{
    string ProviderName { get; }
    string Issuer { get; }
    Task<Result<string>> CreateUserAsync(string identifier, string password, Guid doorlistUserId, CancellationToken cancellationToken);
    Task<Result<string>> DeleteUserAsync(string externalId, CancellationToken cancellationToken);
}