namespace VitaLink.Platform.API.Notification.Domain.Model.Commands;

/// <summary>
///     Command to send an email notification about an alert to a recipient.
/// </summary>
/// <remarks>
///     For now the recipient email and the older adult name travel in the command,
///     until the family members of ElderCare are available.
/// </remarks>
/// <param name="AlertId">Alert that originated the notification (Alerting bounded context).</param>
/// <param name="RecipientId">User that receives the notification.</param>
/// <param name="RecipientEmail">Email address of the recipient.</param>
/// <param name="OlderAdultName">Name of the older adult the alert is about.</param>
/// <param name="Severity">Severity of the alert, as shown in the message.</param>
public record SendAlertNotificationCommand(
    Guid AlertId,
    Guid RecipientId,
    string RecipientEmail,
    string OlderAdultName,
    string Severity);
    