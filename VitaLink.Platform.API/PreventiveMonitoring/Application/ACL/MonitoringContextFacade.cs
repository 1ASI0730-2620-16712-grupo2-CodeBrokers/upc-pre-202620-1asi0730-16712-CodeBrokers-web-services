using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.ACL;

/// <summary>
///     Exposes health record summaries and threshold evaluations to other bounded contexts.
/// </summary>
public class MonitoringContextFacade(IHealthRecordRepository healthRecordRepository,
    IMonitoringRuleRepository monitoringRuleRepository, RiskEvaluationService riskEvaluationService)
    : IMonitoringContextFacade
{
    /// <inheritdoc />
    public async Task<HealthRecordSummary?> FindHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default)
    {
        var record = await healthRecordRepository.FindByIdAsync(new HealthRecordId(healthRecordId), cancellationToken);
        return record is null
            ? null
            : new HealthRecordSummary(record.Id.Value, record.OlderAdultId, record.RecordTypeCode,
                record.Measurement?.Value);
    }

    /// <inheritdoc />
    public async Task<RiskEvaluationSummary?> EvaluateHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default)
    {
        var record = await healthRecordRepository.FindByIdAsync(new HealthRecordId(healthRecordId), cancellationToken);
        if (record is null)
            return null;

        if (record.Measurement is null)
            return new RiskEvaluationSummary(healthRecordId, record.OlderAdultId, MonitoringEvaluationOutcome.NotEvaluable);

        var rules = await monitoringRuleRepository.FindByOlderAdultAsync(record.OlderAdultId, cancellationToken);
        var applicableRules = rules.Where(rule => rule.IsActive && rule.RecordTypeCode == record.RecordTypeCode).ToList();
        if (applicableRules.Count == 0)
            return new RiskEvaluationSummary(healthRecordId, record.OlderAdultId, MonitoringEvaluationOutcome.NoRuleConfigured);

        var violation = applicableRules
            .Where(rule => riskEvaluationService.EvaluateRisk(record, rule, out _))
            .Select(rule => new { Id = rule.Id.Value, Severity = ParseSeverity(rule.SeverityLevel) })
            .OrderBy(rule => rule.Severity)
            .ThenBy(rule => rule.Id)
            .FirstOrDefault();

        return violation is null
            ? new RiskEvaluationSummary(healthRecordId, record.OlderAdultId, MonitoringEvaluationOutcome.WithinLimits)
            : new RiskEvaluationSummary(healthRecordId, record.OlderAdultId, MonitoringEvaluationOutcome.Violated,
                violation.Id, violation.Severity);
    }

    private static SeverityLevel ParseSeverity(string severity) => severity.Trim().ToUpperInvariant() switch
    {
        "HIGH" => SeverityLevel.High,
        "MEDIUM" => SeverityLevel.Medium,
        "LOW" => SeverityLevel.Low,
        _ => throw new InvalidOperationException("The violated monitoring rule has an invalid severity.")
    };
}
