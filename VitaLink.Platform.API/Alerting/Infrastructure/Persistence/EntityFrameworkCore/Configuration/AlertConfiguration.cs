using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping of the <see cref="Alert" /> aggregate and its status history.
/// </summary>
public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts", "alerting");
        // PostgreSQL xmin optimistic concurrency token
        builder.Property(a => a.Version)
            .IsRowVersion();

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => new AlertId(value));

        builder.Property(a => a.Severity).HasConversion(ToDbText<SeverityLevel>()).HasMaxLength(10);
        builder.Property(a => a.Status).HasConversion(ToDbText<AlertStatus>()).HasMaxLength(15);

        // US-15: a health record raises at most one alert
        builder.HasIndex(a => a.HealthRecordId).IsUnique();
        builder.HasIndex(a => new { a.AssignedProviderId, a.Status, a.Severity });
        builder.HasIndex(a => new { a.OlderAdultId, a.RaisedAt });

        builder.OwnsMany(a => a.StatusHistory, history =>
        {
            history.ToTable("alert_status_changes", "alerting");
            history.WithOwner().HasForeignKey("AlertId");
            history.HasKey(c => c.Id);
            history.Property(c => c.Id)
                .HasConversion(id => id.Value, value => new AlertStatusChangeId(value));
            history.Property(c => c.FromStatus).HasConversion(ToDbText<AlertStatus>()).HasMaxLength(15);
            history.Property(c => c.ToStatus).HasConversion(ToDbText<AlertStatus>()).HasMaxLength(15);
        });
        builder.Navigation(a => a.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    // Stores enums as upper snake case text, as in schema.sql (InReview -> IN_REVIEW)
    private static ValueConverter<TEnum, string> ToDbText<TEnum>() where TEnum : struct, Enum =>
        new(value => value.ToString().ToSnakeCase().ToUpperInvariant(),
            text => Enum.Parse<TEnum>(text.Replace("_", string.Empty), true));
}