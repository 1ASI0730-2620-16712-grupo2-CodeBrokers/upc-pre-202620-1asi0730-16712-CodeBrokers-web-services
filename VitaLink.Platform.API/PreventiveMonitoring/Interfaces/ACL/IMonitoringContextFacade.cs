namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

/// <summary>
///     Exposes health record lookups to other bounded contexts.
/// </summary>
public interface IMonitoringContextFacade
{
    /// <summary>
    ///     Finds a summary of the requested health record, or returns null when it does not exist.
    /// </summary>
    Task<HealthRecordSummary?> FindHealthRecordAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Health record data shared with other bounded contexts.
/// </summary>
public record HealthRecordSummary(Guid HealthRecordId, Guid OlderAdultId, string RecordTypeCode,
    decimal? MeasurementValue);
