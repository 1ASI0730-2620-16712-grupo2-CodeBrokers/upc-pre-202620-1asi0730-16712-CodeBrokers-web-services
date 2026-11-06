using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Notification.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core implementation of <see cref="INotificationRepository"/>.
/// </summary>
/// <remarks>
///     Common operations come from <see cref="BaseRepository{TEntity, TId}"/>.
///     It only registers changes; they are committed through the Unit of Work.
/// </remarks>
/// <param name="context">The EF Core database context.</param>
public class NotificationRepository(AppDbContext context)
    : BaseRepository<NotificationAggregate, NotificationId>(context), INotificationRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationAggregate>> FindByRecipientAsync(
        Guid recipientId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationAggregate>()
            .Where(n => n.RecipientId == recipientId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationAggregate>> FindRetryableAsync(
        int maxAttempts, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationAggregate>()
            .Where(n => n.Status == DeliveryStatus.Failed && n.Attempts < maxAttempts)
            .ToListAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<NotificationAggregate?> FindByAlertAndRecipientAsync(
        Guid alertId, Guid recipientId, NotificationChannel channel, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationAggregate>()
            .FirstOrDefaultAsync(
                n => n.RelatedAlertId == alertId && n.RecipientId == recipientId && n.Channel == channel,
                cancellationToken);
    }
    
}