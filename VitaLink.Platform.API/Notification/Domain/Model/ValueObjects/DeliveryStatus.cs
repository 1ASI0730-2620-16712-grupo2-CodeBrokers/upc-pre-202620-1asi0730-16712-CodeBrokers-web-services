namespace VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

/// <summary>
///     Delivery state of a notification sent through an external provider.
/// </summary>
public enum DeliveryStatus
{
    /// <summary>Notification created and waiting to be sent.</summary>
    Pending,

    /// <summary>Notification accepted by the external provider.</summary>
    Sent,

    /// <summary>Last delivery attempt failed; it may be retried.</summary>
    Failed
}