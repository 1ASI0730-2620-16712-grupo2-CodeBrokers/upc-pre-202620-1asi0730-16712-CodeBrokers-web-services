using VitaLink.Platform.API.Notification.Interfaces.REST.Resources;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Interfaces.REST.Transform;

/// <summary>
///     Assembles a <see cref="NotificationResource"/> from a notification.
/// </summary>
public static class NotificationResourceFromEntityAssembler
{
    /// <summary>
    ///     Assembles the resource from the notification aggregate.
    /// </summary>
    /// <param name="entity">The notification.</param>
    /// <returns>The notification resource.</returns>
    public static NotificationResource ToResourceFromEntity(NotificationAggregate entity) =>
        new(entity.Id.Value,
            entity.RecipientId,
            entity.RelatedAlertId,
            entity.Channel.ToString().ToUpperInvariant(),
            entity.Status.ToString().ToUpperInvariant(),
            entity.Attempts,
            entity.CreatedAt,
            entity.SentAt,
            entity.FailureReason);
}