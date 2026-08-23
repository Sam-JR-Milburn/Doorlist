namespace Doorlist.Presentation.Controllers;

using Application.Services.Organisation;
using Application.Services.User;
using Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Create, update and assign membership to organisations - businesses, groups, venue management, etc.
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public class OrganisationController : ApiControllerBase
{
    private readonly ILogger<OrganisationController> _logger;
    
    private readonly IOrganisationService _organisationService;
    
    public OrganisationController(ILogger<OrganisationController> logger, IOrganisationService organisationService)
    {
        _logger = logger;
        
        _organisationService = organisationService;
    }
    
    /// <summary>
    /// Create an organisation.
    /// Will set your account as the owner, and creation will be prevented if you're already a member of an organisation. 
    /// </summary>
    [HttpPost]
    [Route("create")]
    [Authorize(AuthenticationSchemes = "Authentication")]
    public async Task<IActionResult> CreateOrganisation(CancellationToken requestAborted)
    {
        var result = _organisationService.CreateOrganisationAsync(this.CurrentUserId, requestAborted);
        
        //
        // Return DTO? 
        //
        
        return Ok();
    }
    
    /// <summary>
    /// Delete an organisation, if you have ownership rights over it.
    /// </summary>
    [HttpDelete]
    [Route("delete/{id:guid}")]
    // [RequiresRichAuth(ResourceType = "organisation", Actions = new[] { "manage_organisation", "delete" }]
    // [RequiresRichAuth(ResourceType = "admin", Actions = new[] { "manage_users" })]
    public async Task<IActionResult> DeleteOrganisation()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Invite a user to an organisation
    /// </summary>
    [HttpPatch]
    [Route("{id:guid}/invite")]
    // [RequiresRichAuth(ResourceType = "organisation", Actions = new []{ "" })]
    public async Task<IActionResult> InviteUserToOrganisation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Unlink/remove a user from an organisation
    /// </summary>
    [HttpPatch]
    [Route("{id:guid}/withdraw")]
    public async Task<IActionResult> WithdrawUserFromOrganisation()
    {
        throw new NotImplementedException();
    }
    
}