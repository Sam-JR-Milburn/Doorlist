namespace Doorlist.Infrastructure.Persistence.Configurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrganisationInviteConfiguration : IEntityTypeConfiguration<OrganisationInvite>
{
    public void Configure(EntityTypeBuilder<OrganisationInvite> builder)
    {
        builder.ToTable("organisation_invites");
        
        builder.HasKey(t => t.Id);

        // Configure a fixed length and an efficient (b-tree) index for the invitation token
        builder.Property(t => t.InviteToken)
            .HasMaxLength(32)
            .IsFixedLength()
            .IsRequired();
        builder.HasIndex(x => x.InviteToken)
            .IsUnique()
            .HasDatabaseName("ix_organisation_invites_token");
        
        // Store the roles enum
        builder.Property(x => x.Role)
            .HasConversion<string>() // 'Member', 'Admin', 'Owner'
            .HasMaxLength(20) 
            .IsRequired()
            .HasDefaultValue(OrganisationRole.Member);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        builder.Property(x => x.IsRevoked)
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(x => x.AcceptedAt)
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.InvitedUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Organisation>()
            .WithMany()
            .HasForeignKey(x => x.OrganisationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}