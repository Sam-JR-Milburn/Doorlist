namespace Doorlist.Infrastructure.Identity.Options;

using System.Collections;
using System.Reflection;
using System.Text;

/// <summary>
/// Maps the settings from the Keycloak config.
/// </summary>
public class KeycloakSettings
{
    public const string SectionName = "Keycloak";
    
    public string AuthServerUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    
    public string SslRequired { get; set; } = string.Empty;
    public bool VerifySsl { get; set; }
    public string Resource { get; set; } = string.Empty;
    
    public Dictionary<string, string> Credentials { get; set; } = []; 
    public string Issuer => $"{AuthServerUrl.TrimEnd('/')}/realms/{Realm}";

    public string Secret => Credentials.TryGetValue("secret", out var secret) ? secret : string.Empty;
    
    public new string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{nameof(AuthServerUrl)} -> {AuthServerUrl}");
        sb.AppendLine($"{nameof(Realm)} -> {Realm}");
        sb.AppendLine($"{nameof(Resource)} -> {Resource}");
        sb.AppendLine($"{nameof(Secret)} -> {(string.IsNullOrEmpty(Secret) ? "null" : "[PROTECTED]")}");
        return sb.ToString();
    }
}