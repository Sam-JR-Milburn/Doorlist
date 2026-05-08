using Doorlist.Application;
using Doorlist.Infrastructure;
using Doorlist.Presentation.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        Description = "Paste a Keycloak access token obtained from POST /realms/doorlist/protocol/openid-connect/token"
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
        options.MapInboundClaims = false;
        options.Authority = builder.Configuration["Keycloak:BaseUrl"] + "/realms/" + builder.Configuration["Keycloak:Realm"];
        options.Audience = "doorlist-api";
        options.RequireHttpsMetadata = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Keycloak:BaseUrl"] + "/realms/" + builder.Configuration["Keycloak:Realm"],
            ValidateAudience = true,
            ValidAudience = "doorlist-api",
            ValidateLifetime = true,
            RoleClaimType = "roles",
            NameClaimType = "preferred_username",
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
        policy
            .WithOrigins(builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173")
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