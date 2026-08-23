namespace Doorlist.Infrastructure.Security.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class DynamicRichAuthPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public DynamicRichAuthPolicyProvider(IOptions<AuthorizationOptions> options) { }
    
}