namespace Doorlist.Domain.Entities;

/// <summary>
/// Separates user login from user data and allows for multiple providers to provide authorisation. 
/// </summary>
public class UserLogin
{
    public Guid UserId { get; private set; }
    
    // 'Google', 'GitHub', 'Microsoft'
    public string ProviderName { get; private set; }
    public string ProviderKey { get; private set; } // Identifier: sub in the token
    
    // Specific tenant within the provider context. eg. joe@outlook.com is not joe@microsoft_tenanted_corporation.org
    public string Issuer { get; private set; }
    
    public UserLogin() {}
    public UserLogin(Guid userId, string providerName, string providerKey, string issuer)
    {
        UserId = userId;
        ProviderName = providerName;
        ProviderKey = providerKey;
        Issuer = issuer;
    }
}