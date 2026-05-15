namespace Doorlist.Infrastructure.Identity.Options;

/// <summary>
/// Maps settings from the Keycloak config.
/// </summary>
public class KeycloakAdminSettings
{
    public const string SectionName = "Keycloak";
    
    public string Realm { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool VerifySsl { get; set; }

    public class AdminSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
    }
    
    // Maps to the 'Admin' sub-object
    public AdminSettings Admin { get; set; } = new();
}