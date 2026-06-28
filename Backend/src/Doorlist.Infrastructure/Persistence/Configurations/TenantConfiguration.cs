namespace Doorlist.Infrastructure.Persistence.Configurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.TenantName)
            .HasMaxLength(256)
            .IsRequired();
    }
}