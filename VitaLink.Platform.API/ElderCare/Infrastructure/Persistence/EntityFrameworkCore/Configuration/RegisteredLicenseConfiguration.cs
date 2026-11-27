using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping for the simulated registry of professional licenses.
/// </summary>
public class RegisteredLicenseConfiguration : IEntityTypeConfiguration<RegisteredLicense>
{
    public void Configure(EntityTypeBuilder<RegisteredLicense> builder)
    {
        builder.ToTable("license_registry", "elder_care");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id");

        builder.Property(l => l.LicenseNumber)
            .HasColumnName("license_number")
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(l => l.LicenseNumber)
            .IsUnique();

        builder.Property(l => l.HolderName)
            .HasColumnName("holder_name")
            .HasMaxLength(160)
            .IsRequired();
    }
}
