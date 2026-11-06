using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>Maps observations and enforces at most one per alert.</summary>
public class AlertObservationConfiguration : IEntityTypeConfiguration<AlertObservation>
{
    public void Configure(EntityTypeBuilder<AlertObservation> builder)
    {
        builder.ToTable("alert_observations", "alerting", table =>
            table.HasCheckConstraint("ck_alert_observations_text_length", "char_length(text) BETWEEN 1 AND 1000"));
        builder.HasKey(observation => observation.Id);
        builder.Property(observation => observation.Id)
            .HasConversion(id => id.Value, value => new AlertObservationId(value));
        builder.Property(observation => observation.AlertId)
            .HasConversion(id => id.Value, value => new AlertId(value)).IsRequired();
        builder.Property(observation => observation.Text).HasMaxLength(AlertObservation.MaxLength).IsRequired();
        builder.Property(observation => observation.CreatedBy).IsRequired();
        builder.Property(observation => observation.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.HasIndex(observation => observation.AlertId).IsUnique()
            .HasDatabaseName("uq_alert_observations_alert_id");
        builder.HasOne<Alert>().WithMany().HasForeignKey(observation => observation.AlertId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
