using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Translates risk evaluations from Preventive Monitoring into Alerting types.
/// </summary>
public class ExternalMonitoringService(IMonitoringContextFacade monitoringContextFacade)
{
    /// <summary>
    ///     Translates the record evaluation, or returns null when the record does not exist.
    /// </summary>
    public async Task<HealthRecordRisk?> EvaluateHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default)
    {
        var risk = await monitoringContextFacade.EvaluateHealthRecordAsync(healthRecordId, cancellationToken);
        if (risk is null)
            return null;

        var outcome = risk.Outcome switch
        {
            MonitoringEvaluationOutcome.NotEvaluable => HealthRecordRiskOutcome.NotEvaluable,
            MonitoringEvaluationOutcome.NoRuleConfigured => HealthRecordRiskOutcome.NoRuleConfigured,
            MonitoringEvaluationOutcome.WithinLimits => HealthRecordRiskOutcome.WithinLimits,
            MonitoringEvaluationOutcome.Violated => HealthRecordRiskOutcome.Violated,
            _ => throw new InvalidOperationException("Unknown monitoring evaluation outcome.")
        };
        return new HealthRecordRisk(risk.HealthRecordId, risk.OlderAdultId, outcome, risk.MonitoringRuleId, risk.Severity);
    }
}

/// <summary>
///     Result of assessing a health record for alert generation.
/// </summary>
public record HealthRecordRisk(Guid HealthRecordId, Guid OlderAdultId, HealthRecordRiskOutcome Outcome,
    Guid? MonitoringRuleId, SeverityLevel? Severity);

/// <summary>
///     Evaluation outcomes used by Alerting.
/// </summary>
public enum HealthRecordRiskOutcome
{
    NotEvaluable,
    NoRuleConfigured,
    WithinLimits,
    Violated
}
