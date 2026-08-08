namespace Doorlist.Application;

using Microsoft.Extensions.DependencyInjection;
using Doorlist.Application.Services.User;
using Doorlist.Application.Services.Organisation;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrganisationService, OrganisationService>();
        
        return services;
    }
}