using VitaLink.Platform.API.Notification.Domain.Model.Queries;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Application.Services;

/// <summary>
///     Query service for notification read operations.
/// </summary>
public interface INotificationQueryService
{
    /// <summary>
    ///     Gets a notification by its identifier.
    /// </summary>
    /// <param name="query">The get notification by id query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notification, or <c>null</c> if it does not exist.</returns>
    Task<NotificationAggregate?> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists the notifications addressed to a recipient, most recent first.
    /// </summary>
    /// <param name="query">The get notifications by recipient id query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notifications of the recipient; empty if there are none.</returns>
    Task<IReadOnlyList<NotificationAggregate>> Handle(
        GetNotificationsByRecipientIdQuery query, CancellationToken cancellationToken = default);
}