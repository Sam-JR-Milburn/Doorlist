namespace Doorlist.Application.Organisation;

using Domain.Interfaces.User;
using Microsoft.Extensions.Logging;

/// <summary>
/// Stub
/// </summary>
public class OrganisationMembershipService : IOrganisationMembershipService
{
    private readonly ILogger<OrganisationMembershipService> _logger;
    
    private readonly IUserRepository _userRepository;
    //private readonly IOrganisationRepository;
    
    public OrganisationMembershipService(ILogger<OrganisationMembershipService> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }



    // ----
}