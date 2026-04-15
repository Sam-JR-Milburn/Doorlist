using Doorlist.Infrastructure.Security;

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
        // Database service
        services.AddDbContext<DoorlistDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Doorlist"),
                npgsql => npgsql.MigrationsAssembly(typeof(DoorlistDbContext).Assembly.FullName)
            ));

        services.AddHostedService<PostQuantumStartupService>();
        
        return services;
    }
}