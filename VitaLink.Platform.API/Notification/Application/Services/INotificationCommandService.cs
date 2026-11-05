using VitaLink.Platform.API.Notification.Application.Errors;
using VitaLink.Platform.API.Notification.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Application.Services;

/// <summary>
///     Command service for notification operations.
/// </summary>
public interface INotificationCommandService
{
    /// <summary>
    ///     Sends the email notification of an alert to a recipient.
    /// </summary>
    /// <remarks>
    ///     The operation is idempotent: a recipient is notified only once per alert and channel.
    ///     A failed notification is retried with the same record while it has attempts left.
    /// </remarks>
    /// <param name="command">The send alert notification command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The sent notification on success, or the reason of the failure.</returns>
    Task<Result<NotificationAggregate, SendAlertNotificationError>> Handle(
        SendAlertNotificationCommand command, CancellationToken cancellationToken = default);
}