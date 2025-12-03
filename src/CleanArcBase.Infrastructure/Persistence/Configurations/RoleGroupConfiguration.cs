using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Infrastructure.Persistence.Configurations;

public class RoleGroupConfiguration : IEntityTypeConfiguration<RoleGroup>
{
    public void Configure(EntityTypeBuilder<RoleGroup> builder)
    {
        builder.ToTable("RoleGroups");

        builder.HasKey(rg => rg.Id);

        builder.Property(rg => rg.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rg => rg.Description)
            .HasMaxLength(500);

        builder.Property(rg => rg.IsSystem)
            .IsRequired();

        builder.Property(rg => rg.DisplayOrder)
            .IsRequired();

        builder.HasOne(rg => rg.Tenant)
            .WithMany(t => t.RoleGroups)
            .HasForeignKey(rg => rg.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rg => rg.SourceRoleGroup)
            .WithMany()
            .HasForeignKey(rg => rg.SourceRoleGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(rg => rg.TenantId);

        builder.HasIndex(rg => new { rg.TenantId, rg.Name })
            .IsUnique();

        builder.HasIndex(rg => rg.SourceRoleGroupId);
    }
}
