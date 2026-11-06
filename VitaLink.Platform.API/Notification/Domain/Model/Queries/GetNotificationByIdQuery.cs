namespace VitaLink.Platform.API.Notification.Domain.Model.Queries;

/// <summary>
///     Query to get a notification by its identifier.
/// </summary>
/// <param name="NotificationId">Notification identifier.</param>
public record GetNotificationByIdQuery(Guid NotificationId);