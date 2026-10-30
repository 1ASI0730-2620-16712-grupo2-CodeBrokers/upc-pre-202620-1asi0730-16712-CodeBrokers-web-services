namespace VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

/// <summary>
///     Delivery channel used to send a notification to its recipient.
/// </summary>
public enum NotificationChannel
{
    /// <summary>Notification delivered by email.</summary>
    Email,

    /// <summary>Notification delivered by SMS.</summary>
    Sms
}