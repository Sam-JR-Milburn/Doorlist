using Doorlist.Application;
using Doorlist.Infrastructure;

using Doorlist.Presentation.Middleware;
using Doorlist.Presentation.DependencyInjectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Feature components
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCustomSwaggerGen();
builder.Services.AddCustomCorsPolicy(builder.Configuration, builder.Environment);
builder.Services.AddKeycloakJwtAuthentication(builder.Configuration);

// Clean architecture core layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Middleware and error components
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Called after all the setup.
var app = builder.Build();
app.UseExceptionHandler(); // Configure the GlobalExceptionHandler

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Apply migrations if they exist.
    await app.ApplyPendingMigrations();
    
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