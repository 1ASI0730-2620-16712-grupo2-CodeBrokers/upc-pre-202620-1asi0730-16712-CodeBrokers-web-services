using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Transform;

/// <summary>
///     Converts an Alert aggregate into an alert status response resource.
/// </summary>
public static class AlertStatusResourceFromEntityAssembler
{
    public static AlertStatusResource ToResourceFromEntity(Alert alert)
    {
        ArgumentNullException.ThrowIfNull(alert);

        return new AlertStatusResource(
            alert.Id.Value,
            alert.Status.ToString().ToSnakeCase().ToUpperInvariant(),
            alert.LastStatusChangeAt,
            alert.Version);
    }
}