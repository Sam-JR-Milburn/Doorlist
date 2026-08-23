namespace Doorlist.Presentation.DependencyInjectionExtensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Configure any and all presentation layer security config
/// </summary>
public static class SecurityConfigurationExtensions
{
    /// <summary>
    /// Add authentication scheme: Keycloak as IdP
    /// </summary>
    public static IServiceCollection AddAuthenticationScheme(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Authentication", options =>
            {
                // Check for the auth server and realm, don't build into an invalid state 
                var authServerUrl = configuration["Keycloak:AuthServerUrl"]?.TrimEnd('/');
                var realm = configuration["Keycloak:Realm"];
                if (string.IsNullOrEmpty(authServerUrl) || string.IsNullOrEmpty(realm))
                {
                    throw new InvalidConfigurationException(
                        "Couldn't locate either Keycloak:AuthServerUrl or Keycloak:Realm in the config");
                }

                options.MapInboundClaims = false;
                options.Authority = authServerUrl + "/realms/" + realm;
                options.Audience = "doorlist-api";
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = "doorlist-api",
                    ValidateLifetime = true,

                    RoleClaimType = "roles",
                    NameClaimType = "preferred_username",

                    ValidateIssuer = true,
                    ValidIssuers = new[]
                    {
                        $"{authServerUrl}/realms/{realm}",
                        $"{authServerUrl}/realms/{realm}/",

                        // Fallback:running inside the Keycloak container
                        $"https://doorlist_keycloak_server:8443/realms/{realm}",
                        $"https://doorlist_keycloak_server:8443/realms/{realm}/"
                    }
                };
            });
        
        return services;
    }

    /// <summary>
    /// Add authorisation scheme. 
    /// </summary>
    /// <remarks>
    /// Not yet implemented. Intention to use a token exchange system, sourced from the API. 
    /// </remarks>
    public static IServiceCollection AddAuthorisation(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}