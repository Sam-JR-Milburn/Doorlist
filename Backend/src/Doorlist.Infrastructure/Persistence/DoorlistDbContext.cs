namespace Doorlist.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

/// <summary>
/// 
/// </summary>
public class DoorlistDbContext : DbContext
{
    public DoorlistDbContext(DbContextOptions<DoorlistDbContext> options) : base(options) { }
    
    // Tables
    public DbSet<User> Users => Set<User>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<OrganisationInvite> OrganisationInvites => Set<OrganisationInvite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Will apply the configurations from /Persistence/Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoorlistDbContext).Assembly);
    }
}