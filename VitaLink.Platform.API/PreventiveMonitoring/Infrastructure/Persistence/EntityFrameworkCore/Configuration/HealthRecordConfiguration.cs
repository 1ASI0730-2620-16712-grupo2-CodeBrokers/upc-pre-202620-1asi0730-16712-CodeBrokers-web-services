using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

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
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.OlderAdultId).IsRequired();
        builder.Property(r => r.RecordTypeCode).IsRequired().HasMaxLength(50).HasColumnName("record_type_code");
        
        builder.OwnsOne(r => r.Measurement, m =>
        {
            m.Property(p => p.Value).HasColumnName("measurement_value").HasPrecision(18, 2);
        });

        builder.Property(r => r.Note).HasMaxLength(500);
        builder.Property(r => r.Source).HasConversion<string>().IsRequired().HasMaxLength(50);
        builder.Property(r => r.RecordedBy).IsRequired();
        builder.Property(r => r.RecordedAt).IsRequired();

        builder.HasOne<RecordType>()
            .WithMany()
            .HasForeignKey(r => r.RecordTypeCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.OlderAdultId);
        builder.HasIndex(r => r.RecordedAt);
    }
}