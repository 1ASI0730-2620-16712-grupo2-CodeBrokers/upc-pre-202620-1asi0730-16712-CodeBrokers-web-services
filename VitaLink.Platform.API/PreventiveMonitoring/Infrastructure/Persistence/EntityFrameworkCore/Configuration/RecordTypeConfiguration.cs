using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Entity Framework Core configuration for RecordType entity.
/// </summary>
public class RecordTypeConfiguration : IEntityTypeConfiguration<RecordType>
{
    public void Configure(EntityTypeBuilder<RecordType> builder)
    {
        builder.ToTable("record_types", "monitoring");
        builder.HasKey(t => t.Code);
        
        builder.Property(t => t.Code).HasMaxLength(30);
        builder.Property(t => t.DisplayName).HasMaxLength(80).IsRequired();
        builder.Property(t => t.Unit).HasMaxLength(20);
        builder.Property(t => t.MinValidValue).HasPrecision(10, 2);
        builder.Property(t => t.MaxValidValue).HasPrecision(10, 2);

        builder.HasData(
            new { Code = "BLOOD_PRESSURE", DisplayName = "Blood pressure (systolic)", Unit = "mmHg", MinValidValue = 40m, MaxValidValue = 300m, RequiresValue = true, IsActive = true },
            new { Code = "HEART_RATE", DisplayName = "Heart rate", Unit = "bpm", MinValidValue = 20m, MaxValidValue = 250m, RequiresValue = true, IsActive = true },
            new { Code = "GLUCOSE", DisplayName = "Blood glucose", Unit = "mg/dL", MinValidValue = 20m, MaxValidValue = 600m, RequiresValue = true, IsActive = true },
            new { Code = "BODY_TEMPERATURE", DisplayName = "Body temperature", Unit = "°C", MinValidValue = 30m, MaxValidValue = 45m, RequiresValue = true, IsActive = true },
            new { Code = "WEIGHT", DisplayName = "Weight", Unit = "kg", MinValidValue = 1m, MaxValidValue = 300m, RequiresValue = true, IsActive = true },
            new { Code = "FALL_DETECTED", DisplayName = "Fall detected", Unit = (string?)null, MinValidValue = (decimal?)null, MaxValidValue = (decimal?)null, RequiresValue = false, IsActive = true },
            new { Code = "MEDICATION_INTAKE", DisplayName = "Medication intake", Unit = (string?)null, MinValidValue = (decimal?)null, MaxValidValue = (decimal?)null, RequiresValue = false, IsActive = true },
            new { Code = "GENERAL_NOTE", DisplayName = "General note", Unit = (string?)null, MinValidValue = (decimal?)null, MaxValidValue = (decimal?)null, RequiresValue = false, IsActive = true }
        );
    }
}