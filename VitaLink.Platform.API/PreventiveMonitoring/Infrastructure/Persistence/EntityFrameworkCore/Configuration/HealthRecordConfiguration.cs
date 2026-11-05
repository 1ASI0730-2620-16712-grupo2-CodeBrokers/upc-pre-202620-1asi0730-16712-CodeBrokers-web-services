using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Entity Framework Core configuration for HealthRecord aggregate.
/// </summary>
public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.ToTable("health_records", "monitoring", table =>
        {
            table.HasCheckConstraint("ck_hr_source", "source IN ('OLDER_ADULT','FAMILY_CAREGIVER','CARE_PROVIDER')");
            table.HasCheckConstraint("ck_hr_value_or_note", "measurement_value IS NOT NULL OR note IS NOT NULL");
            table.HasCheckConstraint("ck_hr_value_non_negative", "measurement_value IS NULL OR measurement_value >= 0");
        });

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion(id => id.Value, value => new HealthRecordId(value));

        builder.Property(r => r.RecordTypeCode).HasMaxLength(30).IsRequired();
        builder.HasOne<RecordType>().WithMany().HasForeignKey(r => r.RecordTypeCode).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(r => r.Measurement, m =>
        {
            m.WithOwner();
            m.Property<HealthRecordId>("HealthRecordId").HasColumnName("id");
            m.Property(x => x.Value).HasColumnName("measurement_value").HasPrecision(10, 2);
        });

        builder.Property(r => r.Source)
            .HasConversion(v => v.ToString().ToSnakeCase().ToUpperInvariant(),
                           v => Enum.Parse<RecordSource>(v.Replace("_", string.Empty), true))
            .HasMaxLength(20);

        // US-11 and US-17: chronological history by patient and by type
        builder.HasIndex(r => new { r.OlderAdultId, r.RecordedAt });
        builder.HasIndex(r => new { r.OlderAdultId, r.RecordTypeCode, r.RecordedAt });
    }
}