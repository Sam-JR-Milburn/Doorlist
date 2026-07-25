namespace Doorlist.Presentation.Controllers;

using Application.Organisation;
using Application.User;
using Domain.Entities;
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

    /// <summary>
    /// Assign a user to an organisation.
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IActionResult> LinkUserToOrganisation(Guid organisationId, Guid userId)
    {
        return BadRequest();
    }

    /// <summary>
    /// Unassign a user from an organisation.
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IActionResult> UnlinkUserFromOrganisation(Guid organisationId, Guid userId)
    {
        return BadRequest();
    }
}