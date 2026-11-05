using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Translates health records and risk evaluations from Preventive Monitoring into Alerting types.
/// </summary>
public class ExternalMonitoringService(IMonitoringContextFacade monitoringContextFacade)
{
    /// <summary>
    ///     Finds the health record reference, or returns null when the record does not exist.
    /// </summary>
    public async Task<HealthRecordReference?> FindHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default)
    {
        var record = await monitoringContextFacade.FindHealthRecordAsync(healthRecordId, cancellationToken);
        return record is null
            ? null
            : new HealthRecordReference(record.OlderAdultId, record.RecordTypeCode, record.MeasurementValue);
    }

    /// <summary>
    ///     Returns the monitoring rule violated by the record and its severity, if any.
    /// </summary>
    public async Task<HealthRecordRisk?> EvaluateHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default)
    {
        var risk = await monitoringContextFacade.EvaluateHealthRecordAsync(healthRecordId, cancellationToken);
        return risk is null
            ? null
            : new HealthRecordRisk(risk.HealthRecordId, risk.OlderAdultId, risk.MonitoringRuleId, risk.Severity);
    }
}

/// <summary>
///     Health record data used by Alerting.
/// </summary>
public record HealthRecordReference(Guid OlderAdultId, string RecordTypeCode, decimal? MeasurementValue);

/// <summary>
///     Risk detected for a health record, as used by Alerting.
/// </summary>
public record HealthRecordRisk(Guid HealthRecordId, Guid OlderAdultId, Guid MonitoringRuleId,
    SeverityLevel Severity);
