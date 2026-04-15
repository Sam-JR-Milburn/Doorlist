namespace Doorlist.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public class DoorlistDbContext : DbContext
{
    public DoorlistDbContext(DbContextOptions<DoorlistDbContext> options) : base(options)
    {
        
    }


    // tables here

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoorlistDbContext).Assembly);
    }
}