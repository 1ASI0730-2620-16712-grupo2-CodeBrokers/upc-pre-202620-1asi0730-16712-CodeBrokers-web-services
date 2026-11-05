
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.ElderCare.Domain.Model.Aggregates;
using VitaLink.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping for the CareProvider aggregate.
/// </summary>
public class CareProviderConfiguration : IEntityTypeConfiguration<CareProvider>
{
    public void Configure(EntityTypeBuilder<CareProvider> builder)
    {
        builder.ToTable("care_providers", "elder_care");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new CareProviderId(value));

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.OwnsOne(p => p.FullName, name =>
        {
            // The owned value object shares the aggregate's primary key.
            name.Property<CareProviderId>("CareProviderId")
                .HasColumnName("id")
                .HasConversion(
                    id => id.Value,
                    value => new CareProviderId(value));

            name.Property(n => n.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100)
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Navigation(p => p.FullName)
            .IsRequired();

        builder.Property(p => p.Specialty)
            .HasColumnName("specialty")
            .HasConversion(
                specialty => specialty.Value,
                value => new Specialty(value))
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(p => p.Institution)
            .HasColumnName("institution")
            .HasMaxLength(160);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}
