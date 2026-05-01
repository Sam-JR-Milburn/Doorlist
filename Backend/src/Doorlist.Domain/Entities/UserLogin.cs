namespace Doorlist.Domain.Entities;

/// <summary>
/// Separates user login from user data and allows for multiple providers to provide authorisation. 
/// </summary>
public class UserLogin
{
    // Shadow property for the UserId
    
    // 'Google', 'GitHub', 'Microsoft'
    public string ProviderName { get; private set; }
    public string ProviderKey { get; private set; } // Identifyer: sub in the token
    
    // Specific tenant within the provider context. eg. joe@outlook.com is not joe@microsoft_tenanted_corporation.org
    public string Issuer { get; private set; }
    
    public UserLogin() {}
    public UserLogin(string providerName, string providerKey, string issuer)
    {
        ProviderName = providerName;
        ProviderKey = providerKey;
        Issuer = issuer;
    }
}