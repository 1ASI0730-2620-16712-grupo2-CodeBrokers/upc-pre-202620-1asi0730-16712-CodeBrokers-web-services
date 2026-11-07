using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Transform;

/// <summary>
///     Converts an HTTP resource into an alert status update command.
/// </summary>
public static class UpdateAlertStatusCommandFromResourceAssembler
{
    public static UpdateAlertStatusCommand ToCommandFromResource(
        Guid alertId,
        UpdateAlertStatusResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        return new UpdateAlertStatusCommand(
            alertId,
            resource.Status,
            resource.ChangedBy,
            resource.Version);
    }
}