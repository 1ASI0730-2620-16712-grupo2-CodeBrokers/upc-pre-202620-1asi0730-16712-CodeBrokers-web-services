using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.ACL;

/// <summary>
///     Exposes health record summaries and threshold evaluations to other bounded contexts.
/// </summary>
public class MonitoringContextFacade(IHealthRecordRepository healthRecordRepository,
    RiskEvaluationService riskEvaluationService)
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

        var evaluation = await riskEvaluationService.EvaluateAsync(record, cancellationToken);
        var outcome = evaluation.Outcome switch
        {
            RiskEvaluationOutcome.NotEvaluable => MonitoringEvaluationOutcome.NotEvaluable,
            RiskEvaluationOutcome.NoRuleConfigured => MonitoringEvaluationOutcome.NoRuleConfigured,
            RiskEvaluationOutcome.WithinLimits => MonitoringEvaluationOutcome.WithinLimits,
            RiskEvaluationOutcome.Violated => MonitoringEvaluationOutcome.Violated,
            _ => throw new InvalidOperationException("Unknown monitoring evaluation outcome.")
        };

        return new RiskEvaluationSummary(record.Id.Value, record.OlderAdultId, outcome,
            evaluation.Rule?.Id.Value, evaluation.Severity);
    }

}
