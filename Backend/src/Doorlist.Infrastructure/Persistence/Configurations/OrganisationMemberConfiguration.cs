namespace Doorlist.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

/// <summary>
/// Setup the OrganisationMember table
/// </summary>
public class OrganisationMemberConfiguration : IEntityTypeConfiguration<OrganisationMember>
{
    public void Configure(EntityTypeBuilder<OrganisationMember> builder)
    {
        builder.ToTable("organisation_members");
        
        builder.HasKey(x => new { x.UserId, x.OrganisationId });
        
        // Store the roles enum
        builder.Property(x => x.Role)
            .HasConversion<string>() // 'Member', 'Admin', 'Owner'
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue(OrganisationRole.Member);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Organisation>()
            .WithMany()
            .HasForeignKey(x => x.OrganisationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}