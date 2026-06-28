namespace Doorlist.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

/// <summary>
/// Setup the Users table.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateOfBirth)
            .HasColumnType("date")
            .IsRequired();
        
        // Reference the tenancy table
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.TenantId);
        
        // Assists the User logins field encapsulation 
        builder.Metadata.FindNavigation(nameof(User.Logins))!.SetPropertyAccessMode(PropertyAccessMode.Field); 
        
        // Ensure UserLogin cascades the delete on User deletion
        builder.HasMany(x => x.Logins)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .IsRequired() // Login can't exist without a user
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(x => x.Logins)
            .HasField("_logins")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}