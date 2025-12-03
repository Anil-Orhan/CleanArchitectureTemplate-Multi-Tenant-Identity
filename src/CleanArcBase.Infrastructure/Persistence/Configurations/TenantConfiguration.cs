using CleanArcBase.Domain.Entities.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArcBase.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Identifier)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.Identifier)
            .IsUnique();

        builder.Property(t => t.Settings)
            .HasColumnType("jsonb");
    }
}
