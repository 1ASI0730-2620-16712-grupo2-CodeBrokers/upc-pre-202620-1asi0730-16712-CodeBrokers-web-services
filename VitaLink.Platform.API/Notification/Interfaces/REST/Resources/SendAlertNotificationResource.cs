using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.Notification.Interfaces.REST.Resources;

/// <summary>
///     Data required to send the email notification of an alert.
/// </summary>
/// <param name="AlertId">Alert that originated the notification.</param>
/// <param name="RecipientId">User that receives the notification.</param>
/// <param name="RecipientEmail">Email address of the recipient.</param>
/// <param name="OlderAdultName">Name of the older adult the alert is about.</param>
/// <param name="Severity">Severity of the alert.</param>
public record SendAlertNotificationResource(
    [Required] Guid? AlertId,
    [Required] Guid? RecipientId,
    [Required] [EmailAddress] string RecipientEmail,
    [Required] [MaxLength(150)] string OlderAdultName,
    [Required] [MaxLength(20)] string Severity);