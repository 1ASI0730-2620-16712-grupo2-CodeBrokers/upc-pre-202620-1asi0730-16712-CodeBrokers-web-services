using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

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
            table.HasCheckConstraint("ck_rules_threshold", "min_threshold IS NOT NULL OR max_threshold IS NOT NULL");
            table.HasCheckConstraint("ck_rules_min_max", "min_threshold IS NULL OR max_threshold IS NULL OR min_threshold <= max_threshold");
        });

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion(id => id.Value, value => new MonitoringRuleId(value));

        builder.Property(r => r.RecordTypeCode).HasMaxLength(30).IsRequired();
        builder.HasOne<RecordType>().WithMany().HasForeignKey(r => r.RecordTypeCode).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(r => r.Threshold, t =>
        {
            t.WithOwner();
            t.Property<MonitoringRuleId>("MonitoringRuleId").HasColumnName("id");
            t.Property(x => x.Min).HasColumnName("min_threshold").HasPrecision(10, 2);
            t.Property(x => x.Max).HasColumnName("max_threshold").HasPrecision(10, 2);
        });

        builder.Property(r => r.SeverityLevel).HasMaxLength(20).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(200).IsRequired();

        builder.HasIndex(r => new { r.OlderAdultId, r.RecordTypeCode });
    }
}