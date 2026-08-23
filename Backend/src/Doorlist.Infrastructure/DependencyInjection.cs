namespace Doorlist.Infrastructure;

using DependencyInjectionExtensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Doorlist.Application;
using Doorlist.Infrastructure.Security.Services;

public static class DependencyInjection
{
    /// <summary>
    /// Setup the Infra layer 
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        // Convert relative > absolute file config
        services.NormaliseDevelopmentPaths(configuration);
        
        // Startup Services
        services.AddHostedService<ConfigurationValidatorService>();
        services.AddHostedService<PostQuantumStartupService>();
        
        // Setup DB
        services.AddPersistenceInfrastructure(configuration);
        
        // Setup identity provisioners
        services.AddIdentityInfrastructure(configuration);
        
        // ----
        
        return services;
    }
}