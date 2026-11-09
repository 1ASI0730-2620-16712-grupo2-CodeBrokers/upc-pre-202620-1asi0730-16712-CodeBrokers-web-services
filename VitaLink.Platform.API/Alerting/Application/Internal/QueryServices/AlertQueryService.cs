using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Queries;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Domain.Services;

namespace VitaLink.Platform.API.Alerting.Application.Internal.QueryServices;

/// <summary>
///     Handles alert queries through the alert repository.
/// </summary>
public class AlertQueryService(IAlertRepository repository) : IAlertQueryService
{
    /// <inheritdoc />
    public Task<AlertPage> Handle(GetAlertsQuery query, CancellationToken cancellationToken = default) =>
        repository.FindPageAsync(query, cancellationToken);

    /// <inheritdoc />
    public Task<Alert?> Handle(AlertId id, CancellationToken cancellationToken = default) =>
        repository.FindByIdAsync(id, cancellationToken);
}
