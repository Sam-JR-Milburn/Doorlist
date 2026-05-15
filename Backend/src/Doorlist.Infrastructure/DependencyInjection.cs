namespace Doorlist.Infrastructure;

using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


using Doorlist.Application.Identity;
using Keycloak.AuthServices.Sdk;
using Doorlist.Infrastructure.Identity;

using Doorlist.Infrastructure.Security;
using Doorlist.Infrastructure.Persistence;

using Domain.Interfaces.User;
using Identity.Options;
using Npgsql;
using Persistence.Repositories;

public static class DependencyInjection
{
    /// <summary>
    /// Find the backend root path, containing 'Doorlist.slnx'.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">If the backend root path cannot be found.</exception>
    private static string GetBackendRootPath()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        const string anchor = "Doorlist.slnx";
        
        // Recursively look for the current dir containing Doorlist.slnx. Will stop at '/'.
        while (currentDirectory != null && currentDirectory.Parent != null)
        {
            if (currentDirectory.GetFiles(anchor).Any())
            {
                return currentDirectory.FullName;
            }
            currentDirectory = currentDirectory.Parent;
        }
        throw new DirectoryNotFoundException("Couldn't find the backend root.");
    }
    
    /// <summary>
    /// Prepend a relative path with a specified root path.
    /// </summary>
    private static void ConvertRelativePathToAbsolutePath(IConfiguration configuration, string key, string root)
    {
        var relativePath = configuration[key];
        if (!string.IsNullOrEmpty(relativePath) && !Path.IsPathRooted(relativePath))
        {
            configuration[key] = Path.GetFullPath(Path.Combine(root, relativePath));
        }
    }
    
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        // Normalise config before startup services.
        // If this throws an exception it will kill the program gracefully before app.Run().
        try
        {
            var rootPath = GetBackendRootPath();
            Console.WriteLine("Infrastructure - Found backend root path: " + rootPath);

            ConvertRelativePathToAbsolutePath(configuration, "Kestrel:Endpoints:Https:Certificate:Path", rootPath);
            ConvertRelativePathToAbsolutePath(configuration, "OpenSSL:ConfigPath", rootPath);
        }
        catch (DirectoryNotFoundException dnfe)
        {
            Console.WriteLine("Infrastructure - Couldn't find the root path: " + dnfe.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Infrastructure - Generic error in finding the root path: "+ex.Message);
            throw;
        }
        
        // Startup Services
        services.AddHostedService<ConfigurationValidatorService>();
        services.AddHostedService<PostQuantumStartupService>();
        
        // Setup DB
        var db = configuration.GetSection("Databases:DoorlistAPI");
        string? connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = db.GetValue<string>("Host"),
            Port = db.GetValue<int>("Port"),
            Database = db.GetValue<string>("Database"),
            Username = db.GetValue<string>("Username"), // secret
            Password = db.GetValue<string>("Password"), // secret
        }.ConnectionString; 
        services.AddDbContext<DoorlistDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(DoorlistDbContext).Assembly.FullName)
            ));
        
        // Infrastructure persistence
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserLoginRepository, UserLoginRepository>();
        
        // Register the Keycloak client connector
        services.AddKeycloakAdminHttpClient(options =>
            {
                options.AuthServerUrl = configuration.GetValue<string>("Keycloak:BaseUrl");
                options.Realm = configuration.GetValue<string>("Keycloak:Realm")!;
                options.Resource = configuration.GetValue<string>("Keycloak:Resource")!;
                options.SslRequired = configuration.GetValue<string>("Keycloak:VerifySsl")!;
            }
        );
        services.Configure<KeycloakAdminClientOptions>(configuration.GetSection(KeycloakAdminSettings.SectionName));
        services.AddScoped<IIdentityProvisionerService, KeycloakProvisionerService>();
        
        return services;
    }
}