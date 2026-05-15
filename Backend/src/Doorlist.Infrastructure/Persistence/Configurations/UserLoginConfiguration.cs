namespace Doorlist.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

/// <summary>
/// Tracks user identity against an external provider and links to a User object.
/// </summary>
public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable("user_logins");
        
        // Each user can use one Microsoft account, one Google account, one Keycloak account, etc. 
        builder.HasKey(x => new { x.UserId, x.ProviderName });

        builder.Property(x => x.UserId).IsRequired(); // FK to Users.

        builder.Property(x => x.ProviderName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.ProviderKey)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.Issuer)
            .IsRequired()
            .HasMaxLength(256);
        
        // Reverse lookup indexing
        builder.HasIndex("ProviderName", "ProviderKey");
    }
}