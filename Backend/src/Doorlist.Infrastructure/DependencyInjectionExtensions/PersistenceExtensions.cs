namespace Doorlist.Infrastructure.DependencyInjectionExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Doorlist.Application;
using Doorlist.Domain.Interfaces.User;
using Doorlist.Infrastructure.Persistence;
using Doorlist.Infrastructure.Persistence.Repositories;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
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
        
        // UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Entity persistence
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserLoginRepository, UserLoginRepository>();
        
        return services;
    }
}