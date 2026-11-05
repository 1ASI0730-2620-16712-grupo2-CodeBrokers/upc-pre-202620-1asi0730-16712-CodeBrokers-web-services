using VitaLink.Platform.API.Notification.Application.Services;
using VitaLink.Platform.API.Notification.Domain.Model.Queries;
using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Notification.Domain.Repositories;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Application.Internal.QueryServices;

/// <summary>
///     Handles the read queries of the Notification bounded context.
/// </summary>
/// <param name="notificationRepository">Notification repository.</param>
public class NotificationQueryService(INotificationRepository notificationRepository) : INotificationQueryService
{
    /// <inheritdoc />
    public async Task<NotificationAggregate?> Handle(
        GetNotificationByIdQuery query, CancellationToken cancellationToken = default)
    {
        if (query.NotificationId == Guid.Empty) return null;

        return await notificationRepository.FindByIdAsync(
            new NotificationId(query.NotificationId), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationAggregate>> Handle(
        GetNotificationsByRecipientIdQuery query, CancellationToken cancellationToken = default)
    {
        return await notificationRepository.FindByRecipientAsync(query.RecipientId, cancellationToken);
    }
}