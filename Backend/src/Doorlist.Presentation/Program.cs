using Doorlist.Application;
using Doorlist.Infrastructure;
using Doorlist.Infrastructure.Persistence;
using Doorlist.Presentation.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Presentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Check for the auth server and realm, don't build into an invalid state 
        var authServerUrl = builder.Configuration["Keycloak:AuthServerUrl"]?.TrimEnd('/');
        var realm = builder.Configuration["Keycloak:Realm"];
        if (string.IsNullOrEmpty(authServerUrl) || string.IsNullOrEmpty(realm))
        {
            throw new InvalidConfigurationException("Couldn't locate either Keycloak:AuthServerUrl or Keycloak:Realm in the config");
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("user"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
});

// Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DoorlistFrontend", policy =>
    {
        var allowedOrigins = new List<string>
        {
            builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173"
        };

        if (builder.Environment.IsDevelopment())
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

// Called after all the setup.
var app = builder.Build();
app.UseExceptionHandler(); // Configure the GlobalExceptionHandler

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Apply migrations if they exist.
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DoorlistDbContext>();
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
        if (pendingMigrations.Any())
        {
            Console.WriteLine($"Infrastructure migrations - found {pendingMigrations.Count()} pending migrations. Applying... ");
            await context.Database.MigrateAsync();
            Console.WriteLine("Infrastructure database migrated successfully.");
        }
        else
        {
            Console.WriteLine("Infrastructure database is up to date.");
        }
        
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "An error occurred while migrating the database.");
        throw; // Crash in dev.
    }
    
} else if (app.Environment.IsProduction())
{
    // production key vault secrets here
}

app.UseHttpsRedirection();
app.UseCors("DoorlistFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();