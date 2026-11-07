using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.Alerting.Application.Results;

/// <summary>
///     Contains the alert associated with a health record and whether this request created it.
/// </summary>
public record AlertEvaluationResult(Guid HealthRecordId, Alert? Alert, bool Created);
