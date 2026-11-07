namespace VitaLink.Platform.API.Notification.Application.Errors;

/// <summary>
///     Expected errors when sending an alert notification.
/// </summary>
public enum SendAlertNotificationError
{
    /// <summary>
    ///     The recipient was already notified about this alert.
    /// </summary>
    AlreadyNotified,

    /// <summary>
    ///     The notification failed and has no delivery attempts left.
    /// </summary>
    MaxAttemptsReached,

    /// <summary>
    ///     The email provider could not deliver the message; the attempt was saved to be retried.
    /// </summary>
    DeliveryFailed,

    /// <summary>
    ///     An unexpected error occurred while sending the notification.
    /// </summary>
    UnexpectedError
}
    