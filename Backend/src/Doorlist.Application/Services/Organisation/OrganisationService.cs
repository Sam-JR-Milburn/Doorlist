namespace Doorlist.Application.Services.Organisation;

using Domain.Utility;

/// <summary>
/// Stub
/// </summary>
public class OrganisationService : IOrganisationService
{
    public async Task<Result<Boolean>> CreateOrganisationAsync(Guid userId, CancellationToken requestAborted)
    {
        throw new NotImplementedException();
    }
}