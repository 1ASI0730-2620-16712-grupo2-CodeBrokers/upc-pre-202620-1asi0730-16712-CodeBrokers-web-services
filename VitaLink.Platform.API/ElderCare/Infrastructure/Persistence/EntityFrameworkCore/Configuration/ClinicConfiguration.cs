using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping for the Clinic aggregate.
/// </summary>
public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("clinics", "elder_care");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new ClinicId(value));

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(160)
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.District)
            .HasColumnName("district")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}
