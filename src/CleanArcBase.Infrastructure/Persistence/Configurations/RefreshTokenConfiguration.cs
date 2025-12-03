using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        builder.HasIndex(rt => rt.Token);
        builder.HasIndex(rt => rt.UserId);
    }
}
