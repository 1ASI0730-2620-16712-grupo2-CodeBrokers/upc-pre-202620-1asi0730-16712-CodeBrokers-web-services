using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

/// <summary>
///     Exposes health record lookups and risk evaluation to other bounded contexts.
/// </summary>
public interface IMonitoringContextFacade
{
    /// <summary>
    ///     Finds a summary of the requested health record, or returns null when it does not exist.
    /// </summary>
    Task<HealthRecordSummary?> FindHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Evaluates an existing record, or returns null when it does not exist.
    /// </summary>
    Task<RiskEvaluationSummary?> EvaluateHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Health record data shared with other bounded contexts.
/// </summary>
public record HealthRecordSummary(Guid HealthRecordId, Guid OlderAdultId, string RecordTypeCode,
    decimal? MeasurementValue);

/// <summary>
///     Describes the evaluation and identifies the highest-severity violated rule, when applicable.
/// </summary>
public record RiskEvaluationSummary(Guid HealthRecordId, Guid OlderAdultId, MonitoringEvaluationOutcome Outcome,
    Guid? MonitoringRuleId = null, SeverityLevel? Severity = null);

/// <summary>
///     Possible outcomes of evaluating an existing health record.
/// </summary>
public enum MonitoringEvaluationOutcome
{
    NotEvaluable,
    NoRuleConfigured,
    WithinLimits,
    Violated
}
