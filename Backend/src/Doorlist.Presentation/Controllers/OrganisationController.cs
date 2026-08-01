namespace Doorlist.Presentation.Controllers;

using Application.Organisation;
using Application.Entities.User;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Create, update and assign membership to organisations - businesses, groups, venue management, etc.
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public class OrganisationController : ControllerBase
{
    private readonly ILogger<OrganisationController> _logger;
    
    private readonly IOrganisationService _organisationService;
    private readonly IOrganisationMembershipService _organisationMembershipService;
    
    public OrganisationController(ILogger<OrganisationController> logger, IOrganisationService organisationService, IOrganisationMembershipService organisationMembershipService)
    {
        _logger = logger;
        
        _organisationService = organisationService;
        _organisationMembershipService = organisationMembershipService;
    }
    
    /**
     * Link/join flow: 
     * Invite link: organisation member with 'manage_members' permission can generate an invite link targeting a specific user id.
     * It has to be accepted by that user willingly - using their own perm flows.
     *
     * Unlink/exit flow:
     * Either someone with manage_members on that organisation (who isn't the owner) or the user themselves - user guid with 'self' action 
     * ----
     * ----
     */
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    /*
    // This should be a site admin level flow 
     
    /// <summary>
    /// Assign a user to an organisation.
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("linkUser")]
    public async Task<IActionResult> LinkUserToOrganisation(Guid organisationId, Guid userId)
    {
        return BadRequest();
    }
    */
    
    
    
    
    
    
    //public async Task<IActionResult> Get()
    
    
    
    
    
    
    
    
    
    
    
    
    
    // Requires either the user themselves or an admin with manage_members permission
    
    /// <summary>
    /// Unassign a user from an organisation.
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("unlinkUser")]
    // Requires URN perms for that particular user or site read_users
    public async Task<IActionResult> UnlinkUserFromOrganisation(Guid organisationId, Guid userId)
    {
        return BadRequest();
    }
}