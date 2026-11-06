using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

namespace VitaLink.Platform.API.Alerting.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Translates health record summaries from Preventive Monitoring into Alerting references.
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
}

/// <summary>
///     Health record data used by Alerting.
/// </summary>
public record HealthRecordReference(Guid OlderAdultId, string RecordTypeCode, decimal? MeasurementValue);
