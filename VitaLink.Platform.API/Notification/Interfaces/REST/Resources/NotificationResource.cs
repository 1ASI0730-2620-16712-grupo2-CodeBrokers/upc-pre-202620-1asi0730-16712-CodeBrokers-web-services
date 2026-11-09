namespace VitaLink.Platform.API.Notification.Interfaces.REST.Resources;

/// <summary>
///     Notification data returned by the API.
/// </summary>
/// <remarks>
///     The recipient email and the message content are not exposed.
/// </remarks>
/// <param name="Id">Notification identifier.</param>
/// <param name="RecipientId">User that receives the notification.</param>
/// <param name="RelatedAlertId">Alert that originated the notification, if any.</param>
/// <param name="Channel">Delivery channel (EMAIL or SMS).</param>
/// <param name="Status">Delivery status (PENDING, SENT or FAILED).</param>
/// <param name="Attempts">Number of delivery attempts.</param>
/// <param name="CreatedAt">Creation date.</param>
/// <param name="SentAt">Date the provider accepted the message, if sent.</param>
/// <param name="FailureReason">Reason of the last failure, if failed.</param>
public record NotificationResource(
    Guid Id,
    Guid RecipientId,
    Guid? RelatedAlertId,
    string Channel,
    string Status,
    int Attempts,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SentAt,
    string? FailureReason);