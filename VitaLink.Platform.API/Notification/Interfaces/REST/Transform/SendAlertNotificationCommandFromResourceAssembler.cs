using VitaLink.Platform.API.Notification.Domain.Model.Commands;
using VitaLink.Platform.API.Notification.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Notification.Interfaces.REST.Transform;

/// <summary>
///     Assembles a <see cref="SendAlertNotificationCommand"/> from a <see cref="SendAlertNotificationResource"/>.
/// </summary>
public static class SendAlertNotificationCommandFromResourceAssembler
{
    /// <summary>
    ///     Assembles the command from the validated resource.
    /// </summary>
    /// <param name="resource">The send alert notification resource.</param>
    /// <returns>The send alert notification command.</returns>
    public static SendAlertNotificationCommand ToCommandFromResource(SendAlertNotificationResource resource) =>
        new(resource.AlertId!.Value,
            resource.RecipientId!.Value,
            resource.RecipientEmail,
            resource.OlderAdultName,
            resource.Severity);
}