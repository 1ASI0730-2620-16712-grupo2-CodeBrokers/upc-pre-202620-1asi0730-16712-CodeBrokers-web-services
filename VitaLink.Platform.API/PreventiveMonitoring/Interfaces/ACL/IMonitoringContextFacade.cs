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
    ///     Returns the highest-severity rule violated by the health record, if any.
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
///     Identifies the violated monitoring rule and its severity.
/// </summary>
public record RiskEvaluationSummary(Guid HealthRecordId, Guid OlderAdultId, Guid MonitoringRuleId,
    SeverityLevel Severity);
