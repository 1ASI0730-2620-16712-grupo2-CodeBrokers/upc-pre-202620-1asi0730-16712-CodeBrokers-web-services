using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Entity Framework Core configuration for MonitoringRule aggregate.
/// </summary>
public class MonitoringRuleConfiguration : IEntityTypeConfiguration<MonitoringRule>
{
    public void Configure(EntityTypeBuilder<MonitoringRule> builder)
    {
        builder.ToTable("monitoring_rules", "monitoring", table =>
        {
            table.HasCheckConstraint("ck_mr_severity", "severity IN ('HIGH','MEDIUM','LOW')");
            table.HasCheckConstraint("ck_mr_at_least_one_bound", "min_value IS NOT NULL OR max_value IS NOT NULL");
            table.HasCheckConstraint("ck_mr_bounds_consistent", "min_value IS NULL OR max_value IS NULL OR min_value <= max_value");
        });

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion(id => id.Value, value => new MonitoringRuleId(value));

        builder.Property(r => r.RecordTypeCode).HasMaxLength(30).IsRequired();
        builder.HasOne<RecordType>().WithMany().HasForeignKey(r => r.RecordTypeCode).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(r => r.Threshold, t =>
        {
            t.WithOwner();
            t.Property<MonitoringRuleId>("MonitoringRuleId").HasColumnName("id");
            t.Property(x => x.Min).HasColumnName("min_value").HasPrecision(10, 2);
            t.Property(x => x.Max).HasColumnName("max_value").HasPrecision(10, 2);
        });

        builder.Property(r => r.Severity).HasConversion(ToDbText<SeverityLevel>()).HasMaxLength(10).IsRequired();

        // schema.sql: one active rule per older adult, record type and severity
        builder.HasIndex(r => new { r.OlderAdultId, r.RecordTypeCode, r.Severity })
            .IsUnique()
            .HasFilter("is_active")
            .HasDatabaseName("uq_mr_active_rule");
    }

    // Stores enums as upper snake case text, as in schema.sql
    private static ValueConverter<TEnum, string> ToDbText<TEnum>() where TEnum : struct, Enum =>
        new(value => value.ToString().ToSnakeCase().ToUpperInvariant(),
            text => Enum.Parse<TEnum>(text.Replace("_", string.Empty), true));
}