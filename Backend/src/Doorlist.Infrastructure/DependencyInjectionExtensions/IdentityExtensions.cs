namespace Doorlist.Infrastructure.DependencyInjectionExtensions;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Doorlist.Application.Identity;
using Doorlist.Infrastructure.Identity;
using Doorlist.Infrastructure.Identity.Options;

using Duende.AccessTokenManagement;

using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Sdk.Admin;

public static class IdentityExtensions
{
    /// <summary>
    /// Configure Keycloak, among other providers
    /// </summary>
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Keycloak: Register the realm details for DI.
        var keycloakSection = configuration.GetSection("Keycloak");
        services.Configure<KeycloakSettings>(keycloakSection);
        var keycloakSettings = keycloakSection.Get<KeycloakSettings>() ?? throw new InvalidOperationException("Could not bind KeycloakSettings configuration section.");
        
        // Keycloak: Establish handlers for incoming authentication
        services.AddAuthorization().AddKeycloakAuthorization();
        
        // Keycloak: Use the Duende OpenID Connect library for token management
        services.AddDistributedMemoryCache();
        services.AddClientCredentialsTokenManagement().AddClient("keycloak.admin.token", client =>
        {
            client.TokenEndpoint    = new Uri($"{keycloakSettings.AuthServerUrl}/realms/{keycloakSettings.Realm}/protocol/openid-connect/token");
            client.ClientId         = ClientId.Parse(keycloakSettings.Resource); 
            client.ClientSecret     = ClientSecret.Parse(keycloakSettings.Secret);
        });
        
        // Keycloak: Configure the outbound admin client
        services.AddKeycloakAdminHttpClient(options => 
            { 
                options.AuthServerUrl       = keycloakSettings.AuthServerUrl;
                options.Realm               = keycloakSettings.Realm;
                options.Resource            = keycloakSettings.Resource;
                options.SslRequired         = keycloakSettings.SslRequired;
                options.Credentials         = new KeycloakClientInstallationCredentials
                {
                    Secret  = keycloakSettings.Secret,
                };
            })
            .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("keycloak.admin.token"));
        
        services.AddScoped<IIdentityProvisionerService, KeycloakProvisionerService>();

        return services;
    }
}