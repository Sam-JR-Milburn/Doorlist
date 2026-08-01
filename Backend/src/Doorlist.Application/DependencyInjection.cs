namespace Doorlist.Application;

using Microsoft.Extensions.DependencyInjection;
using Doorlist.Application.Entities.User;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application services
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}