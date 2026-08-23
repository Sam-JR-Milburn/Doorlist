namespace Doorlist.Infrastructure.Security.Authorization;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// RFC 9396 Rich Authorization Requests
/// Defines the controller annotation - targets a resource type and actions are evaluated in a logical OR. 
/// </summary>
/// <example>
/// [RequiresRichAuth(ResourceType = "organisation", Actions = new[] { "manage_organisation", "delete" }]
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequiresRichAuth : AuthorizeAttribute
{
    public string ResourceType { get; init; } = string.Empty;
    public string[] Actions { get; init; } = Array.Empty<string>();
    public bool IsLocationBound { get; init; } = true;

    public new string? Policy
    {
        get
        {
            if (string.IsNullOrEmpty(ResourceType) || Actions.Length == 0) return null;
            
            string joinedActions = string.Join(',', Actions.Select(action => action.Trim().ToLowerInvariant()));
            return $"RAR:{ResourceType}:{IsLocationBound}:{joinedActions}";
        }
        set => base.Policy = value;
    }

    public RequiresRichAuth() {}
    
    public RequiresRichAuth(string resourceType, params string[] actions)
    {
        ResourceType = resourceType;
        Actions = actions;
    }
    
}

/// <summary>
/// Contains data to satisfy the RequiresRichAuthAttribute policy. 
/// </summary>
public class RequiresRichAuthRequirement : IAuthorizationRequirement
{
    public string ResourceType { get; }
    public IReadOnlyList<string> Actions { get; }
    public bool IsLocationBound { get; }

    public RequiresRichAuthRequirement(string resourceType, IEnumerable<string> actions, bool isLocationBound)
    {
        ResourceType = resourceType;
        Actions = actions.ToList().AsReadOnly();
        IsLocationBound = isLocationBound;
    }
    
}