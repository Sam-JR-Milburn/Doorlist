namespace Doorlist.Presentation.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;

/// <summary>
/// Orchestrate miscellaneous presentation layer config
/// </summary>
public static class PresentationConfigurationExtensions
{
    /// <summary>
    /// Set up the Swagger API definitions
    /// </summary>
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Doorlist API", Version = "v1" });
    
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste a Keycloak access token obtained from POST /realms/doorlist/protocol/openid-connect/token",
            });
    
            // UI Option
            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    []
                }
            });
        });
    }
    
    /// <summary>
    /// Configure the API's CORS policy against the appsettings configuration 
    /// </summary>
    public static IServiceCollection AddCustomCorsPolicy(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy("DoorlistFrontend", policy =>
            {
                var allowedOrigins = new List<string>
                {
                    configuration["Frontend:BaseUrl"] ?? "http://localhost:5173"
                };

                if (environment.IsDevelopment())
                {
                    allowedOrigins.Add("null"); // null: Allow file:// access for debugging 
                }
        
                policy
                    .WithOrigins(allowedOrigins.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // required for SignalR
            });
        });
    }
}