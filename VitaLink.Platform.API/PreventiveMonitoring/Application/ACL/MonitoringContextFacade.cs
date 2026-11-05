using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.ACL;

/// <summary>
///     Maps health records to summaries for other bounded contexts.
/// </summary>
public class MonitoringContextFacade(IHealthRecordRepository healthRecordRepository) : IMonitoringContextFacade
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
}
