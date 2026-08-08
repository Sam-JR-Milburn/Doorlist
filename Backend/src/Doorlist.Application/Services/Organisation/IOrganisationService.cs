namespace Doorlist.Application.Services.Organisation;

using Domain.Utility;

/// <summary>
/// Stub
/// </summary>
public interface IOrganisationService
{
    public Task<Result<Boolean>> CreateOrganisationAsync(CancellationToken requestAborted);
}