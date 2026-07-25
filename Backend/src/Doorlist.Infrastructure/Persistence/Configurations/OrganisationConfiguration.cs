namespace Doorlist.Infrastructure.Persistence.Configurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ToTable("organisations");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.OrganisationName)
            .HasMaxLength(256)
            .IsRequired();
    }
}