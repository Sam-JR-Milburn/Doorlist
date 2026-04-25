using Doorlist.Infrastructure.Security;
using Npgsql;

namespace Doorlist.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;


using Doorlist.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
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
            Username = db.GetValue<string>("Username"),
            Password = db.GetValue<string>("Password"),
        }.ConnectionString; 
        services.AddDbContext<DoorlistDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(DoorlistDbContext).Assembly.FullName)
            ));
        
        return services;
    }
}