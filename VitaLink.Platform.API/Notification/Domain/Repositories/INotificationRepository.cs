using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Domain.Repositories;

/// <summary>
///     Repository contract for the Notification aggregate.
/// </summary>
/// <remarks>
///     It does not persist changes by itself; changes are committed through the Unit of Work.
/// </remarks>
public interface INotificationRepository
{
    /// <summary>
    ///     Adds a new notification to the repository.
    /// </summary>
    /// <param name="notification">Notification to add.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    Task AddAsync(NotificationAggregate notification, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Marks an existing notification as modified.
    /// </summary>
    /// <param name="notification">Notification to update.</param>
    void Update(NotificationAggregate notification);

    /// <summary>
    ///     Finds a notification by its identifier.
    /// </summary>
    /// <param name="id">Notification identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The notification if found; otherwise, null.</returns>
    Task<NotificationAggregate?> FindByIdAsync(NotificationId id, CancellationToken cancellationToken = default);

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