namespace Doorlist.Infrastructure.Security.Authorization;

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Implements the logic of the RequiresRichAuth annotation
/// </summary>
public class RichAuthPermissionHandler : AuthorizationHandler<RequiresRichAuthRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string UrnPrefix = "urn:doorlist";

    public RichAuthPermissionHandler(IHttpContextAccessor httpContextAssessor)
    {
        _httpContextAccessor = httpContextAssessor;
    }
    
    /// <summary>
    /// TODO: Commentary
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequiresRichAuthRequirement requirement)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return Task.CompletedTask;

        string? rawRichAuthClaim = context.User.FindFirst("authorization_details")?.Value;
        if (string.IsNullOrWhiteSpace(rawRichAuthClaim)) return Task.CompletedTask;
        
        // Parse the 'authorization_details'.
        List<RichAuthDetail>? richAuthDetails;
        try
        {
            richAuthDetails = JsonSerializer.Deserialize<List<RichAuthDetail>>(rawRichAuthClaim);
        }
        catch
        {
            return Task.CompletedTask;
        }
        if (richAuthDetails == null || !richAuthDetails.Any()) return Task.CompletedTask;
        
        // Requirement to match the route controller and route id against the details of the URN. 
        string? targetResourceGuid;
        // Bound: eg. /api/User/f47ac10b-58cc-4372-a567-0e02b2c3d479
        if (requirement.IsLocationBound)
        {
            var routeData = httpContext.GetRouteData();
            targetResourceGuid =
                routeData.Values["id"]?.ToString() ?? 
                routeData.Values[$"{requirement.ResourceType}Id"]?.ToString();
        }
        // Unbound: eg. /api/Organisations/create
        else
        {
            targetResourceGuid = 
                context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                context.User.FindFirst("sub")?.Value;
        }
        if (targetResourceGuid == null) return Task.CompletedTask;
        
        // Evaluate a complete URN, determine if the authorization_details from the request is matching the annotation's auth requirements.  
        string expectedUrn = $"{UrnPrefix}:{requirement.ResourceType}:{targetResourceGuid}";
        // Looking for *any* permission
        bool authorised = richAuthDetails.Any(authDetail => 
            authDetail.Locations.Any(location => location.Equals(expectedUrn, StringComparison.OrdinalIgnoreCase)) &&
            authDetail.Actions.Any(action => requirement.Actions.Contains(action, StringComparer.OrdinalIgnoreCase)));
        
        if (authorised) context.Succeed(requirement);
        return Task.CompletedTask;
    }
    
}