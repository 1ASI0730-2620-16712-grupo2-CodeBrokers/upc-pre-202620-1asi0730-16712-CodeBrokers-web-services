
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping for the OlderAdult aggregate.
/// </summary>
public class OlderAdultConfiguration : IEntityTypeConfiguration<OlderAdult>
{
    public void Configure(EntityTypeBuilder<OlderAdult> builder)
    {
        builder.ToTable("older_adults", "elder_care");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new OlderAdultId(value));

        builder.Property(o => o.UserId)
            .HasColumnName("user_id");

        builder.HasIndex(o => o.UserId)
            .IsUnique();

        builder.OwnsOne(o => o.FullName, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100)
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100)
                .IsRequired();
            name.Property<OlderAdultId>("OlderAdultId")
                .HasColumnName("id")
                .HasConversion(
                    id => id.Value,
                    value => new OlderAdultId(value));
        });

        builder.Navigation(o => o.FullName)
            .IsRequired();

        builder.Property(o => o.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date")
            .IsRequired();

        builder.OwnsOne(o => o.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("address_street")
                .HasMaxLength(160);

            address.Property(a => a.District)
                .HasColumnName("address_district")
                .HasMaxLength(80);

            address.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(80);
            address.Property<OlderAdultId>("OlderAdultId")
                .HasColumnName("id")
                .HasConversion(
                    id => id.Value,
                    value => new OlderAdultId(value));
        });

        builder.OwnsOne(o => o.EmergencyContact, contact =>
        {
            contact.Property(c => c.Name)
                .HasColumnName("emergency_contact_name")
                .HasMaxLength(160);

            contact.Property(c => c.PhoneNumber)
                .HasColumnName("emergency_contact_phone")
                .HasMaxLength(20);
            contact.Property<OlderAdultId>("OlderAdultId")
                .HasColumnName("id")
                .HasConversion(
                    id => id.Value,
                    value => new OlderAdultId(value));
        });

        builder.Property(o => o.AssignedProviderId)
            .HasColumnName("assigned_provider_id");

        builder.HasIndex(o => o.AssignedProviderId);

        builder.Property(o => o.IsMonitoringActive)
            .HasColumnName("is_monitoring_active")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}
