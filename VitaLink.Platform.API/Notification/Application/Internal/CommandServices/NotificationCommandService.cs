using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Notification.Application.Errors;
using VitaLink.Platform.API.Notification.Application.Services;
using VitaLink.Platform.API.Notification.Domain.Model.Commands;
using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Notification.Domain.Repositories;
using VitaLink.Platform.API.Notification.Domain.Services;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Application.Internal.CommandServices;

/// <summary>
///     Handles the commands of the Notification bounded context.
/// </summary>
/// <param name="notificationRepository">Notification repository.</param>
/// <param name="emailSenderService">Email provider port.</param>
/// <param name="unitOfWork">Unit of work that commits the changes.</param>
/// <param name="localizer">Localizer for the message texts.</param>
/// <param name="logger">Logger.</param>
public class NotificationCommandService(
    INotificationRepository notificationRepository,
    IEmailSenderService emailSenderService,
    IUnitOfWork unitOfWork,
    IStringLocalizer<SharedResource> localizer,
    ILogger<NotificationCommandService> logger) : INotificationCommandService
{
    /// <inheritdoc />
    public async Task<Result<NotificationAggregate, SendAlertNotificationError>> Handle(
        SendAlertNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.FindByAlertAndRecipientAsync(
            command.AlertId, command.RecipientId, NotificationChannel.Email, cancellationToken);

        if (notification is not null)
        {
            if (notification.IsDelivered())
            {
                logger.LogInformation("Alert {AlertId} was already notified to recipient {RecipientId}",
                    command.AlertId, command.RecipientId);
                return Fail(SendAlertNotificationError.AlreadyNotified);
            }

            if (notification.Status == DeliveryStatus.Failed && !notification.CanRetry())
            {
                logger.LogWarning("Notification {NotificationId} reached the maximum number of attempts",
                    notification.Id);
                return Fail(SendAlertNotificationError.MaxAttemptsReached);
            }
        }
        else
        {
            notification = new NotificationAggregate(
                command.RecipientId, NotificationChannel.Email, BuildAlertContent(command));
            notification.LinkToAlert(command.AlertId);
            await notificationRepository.AddAsync(notification, cancellationToken);
        }

        var delivered = await TrySendAsync(notification, command.RecipientEmail, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Concurrent notification of alert {AlertId} to recipient {RecipientId}",
                command.AlertId, command.RecipientId);
            return Fail(SendAlertNotificationError.AlreadyNotified);
        }

        return delivered
            ? new Result<NotificationAggregate, SendAlertNotificationError>.Success(notification)
            : Fail(SendAlertNotificationError.DeliveryFailed);
    }

    private MessageContent BuildAlertContent(SendAlertNotificationCommand command)
    {
        var subject = string.Format(localizer["NotificationAlertEmailSubject"].Value, command.OlderAdultName);
        var body = string.Format(localizer["NotificationAlertEmailBody"].Value,
            command.Severity, command.OlderAdultName);
        return new MessageContent(subject, body);
    }

    private async Task<bool> TrySendAsync(
        NotificationAggregate notification, string recipientEmail, CancellationToken cancellationToken)
    {
        DeliveryReceipt receipt;
        try
        {
            receipt = await emailSenderService.SendAsync(recipientEmail, notification.Content, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            RegisterDeliveryFailure(notification, ex);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            RegisterDeliveryFailure(notification, ex);
            return false;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            RegisterDeliveryFailure(notification, ex);
            return false;
        }

        notification.MarkAsSent(receipt);
        return true;
    }

    private void RegisterDeliveryFailure(NotificationAggregate notification, Exception exception)
    {
        logger.LogWarning(exception, "Email delivery failed for notification {NotificationId}", notification.Id);
        notification.MarkAsFailed(exception.Message);
    }

    private static Result<NotificationAggregate, SendAlertNotificationError> Fail(
        SendAlertNotificationError error) =>
        new Result<NotificationAggregate, SendAlertNotificationError>.Failure(error);
}