using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Domain.Repositories;

/// <summary>
///     Repository contract for the Notification aggregate.
/// </summary>
/// <remarks>
///     Common operations (add, find by id, update, remove and list) come from
///     <see cref="IBaseRepository{TEntity, TId}"/>. Changes are committed through the Unit of Work.
/// </remarks>
public interface INotificationRepository : IBaseRepository<NotificationAggregate, NotificationId>
{
    /// <summary>
    ///     Lists the notifications addressed to a recipient, most recent first.
    /// </summary>
    /// <param name="recipientId">Recipient user identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    Task<IReadOnlyList<NotificationAggregate>> FindByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists failed notifications that still have delivery attempts available.
    /// </summary>
    /// <param name="maxAttempts">Attempt limit.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    Task<IReadOnlyList<NotificationAggregate>> FindRetryableAsync(int maxAttempts, CancellationToken cancellationToken = default);
}