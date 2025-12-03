using CleanArcBase.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArcBase.Infrastructure.Persistence.Configurations;

public class RoleGroupRoleConfiguration : IEntityTypeConfiguration<RoleGroupRole>
{
    public void Configure(EntityTypeBuilder<RoleGroupRole> builder)
    {
        builder.ToTable("RoleGroupRoles");

        builder.HasKey(rgr => new { rgr.RoleGroupId, rgr.RoleId });

        builder.HasOne(rgr => rgr.RoleGroup)
            .WithMany(rg => rg.RoleGroupRoles)
            .HasForeignKey(rgr => rgr.RoleGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rgr => rgr.Role)
            .WithMany()
            .HasForeignKey(rgr => rgr.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rgr => rgr.RoleId);
    }
}
