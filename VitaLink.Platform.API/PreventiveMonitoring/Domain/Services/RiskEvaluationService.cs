using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;

/// <summary>
///     Domain service to evaluate health records against the active monitoring rules of an older adult.
/// </summary>
public class RiskEvaluationService(IMonitoringRuleRepository monitoringRuleRepository)
{
    /// <summary>
    ///     Evaluates a health record. Only active rules for the same older adult and record type are considered.
    ///     If several rules are violated, the one with the highest severity is returned.
    /// </summary>
    public async Task<RiskEvaluationResult> EvaluateAsync(HealthRecord record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (record.Measurement is null)
            return RiskEvaluationResult.NotEvaluable();

        var rules = await monitoringRuleRepository.FindActiveByOlderAdultAndTypeAsync(
            record.OlderAdultId, record.RecordTypeCode, cancellationToken);

        if (rules.Count == 0)
            return RiskEvaluationResult.NoRuleConfigured();

        var value = record.Measurement.Value;
        var mostSevere = rules
            .Where(rule => rule.Threshold.IsViolatedBy(value))
            .OrderByDescending(rule => SeverityRank(rule.Severity))
            .FirstOrDefault();

        return mostSevere is null
            ? RiskEvaluationResult.WithinLimits()
            : RiskEvaluationResult.Violated(mostSevere);
    }

    private static int SeverityRank(SeverityLevel severity) => severity switch
    {
        SeverityLevel.High => 3,
        SeverityLevel.Medium => 2,
        SeverityLevel.Low => 1,
        _ => 0
    };
}
