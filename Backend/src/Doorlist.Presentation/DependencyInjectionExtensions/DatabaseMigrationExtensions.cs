namespace Doorlist.Presentation.DependencyInjectionExtensions;

using Doorlist.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Apply any necessary database config
/// </summary>
public static class DatabaseMigrationExtensions
{
    /// <summary>
    /// Apply migrations if there are any.
    /// </summary>
    public static async Task ApplyPendingMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            var context = serviceProvider.GetRequiredService<DoorlistDbContext>();
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
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(ex, "An error occurred while migrating the database.");
            throw; // Crash in dev.
        }
    }
}