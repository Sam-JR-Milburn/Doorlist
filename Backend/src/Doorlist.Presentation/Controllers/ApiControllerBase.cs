namespace Doorlist.Presentation.Controllers;

using System.Security.Claims;
using System.Text.Json;
using Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Retrieve the sub value from the auth token, which should be the user guid.
    /// </summary>
    protected Guid CurrentUserId
    {
        get
        {
            var subClaim = 
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                User.FindFirst("sub")?.Value;
            
            if (Guid.TryParse(subClaim, out var userId)) return userId;
            throw new UnauthorizedAccessException("Missing/invalid \'sub\' in the received JWT");
        }
    }
    
    /// <summary>
    /// RFC 9396: Grab all of the authorization_details from a request
    /// </summary>
	private List<RichAuthDetail>? _rarDetailsCache;
    protected IReadOnlyList<RichAuthDetail> RarDetails
    {
        get
        {
            if (_rarDetailsCache != null) return _rarDetailsCache;
            var rawRarClaim = User.FindFirst("authorization_details")?.Value;
            if (string.IsNullOrWhiteSpace(rawRarClaim))
            {
                _rarDetailsCache = new List<RichAuthDetail>();
                return _rarDetailsCache;
            }

            // Non-empty 'authorization_details', attempt to parse.
            try
            {
                _rarDetailsCache = JsonSerializer.Deserialize<List<RichAuthDetail>>(rawRarClaim) ?? new List<RichAuthDetail>();
            }
            catch
            {
                _rarDetailsCache = new List<RichAuthDetail>();
            }
            return _rarDetailsCache;
        }
    }
    
    /// <summary>
    /// RFC 9396: Check if a specific action applies for a specific URN 
    /// </summary>
    protected bool HasRarPermission(string locationUrn, string action)
    {
        return RarDetails.Any(
            detail =>
            detail.Locations.Contains(locationUrn, StringComparer.OrdinalIgnoreCase) &&
            detail.Actions.Contains(action, StringComparer.OrdinalIgnoreCase));
    }
    
}