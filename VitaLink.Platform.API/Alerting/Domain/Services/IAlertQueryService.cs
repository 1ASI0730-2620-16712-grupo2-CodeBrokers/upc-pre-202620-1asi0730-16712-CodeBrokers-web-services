using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Queries;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Services;

/// <summary>
///     Provides read-only alert searches and detail queries.
/// </summary>
public interface IAlertQueryService
{
    Task<AlertPage> Handle(GetAlertsQuery query, CancellationToken cancellationToken = default);
    Task<Alert?> Handle(AlertId id, CancellationToken cancellationToken = default);
}
