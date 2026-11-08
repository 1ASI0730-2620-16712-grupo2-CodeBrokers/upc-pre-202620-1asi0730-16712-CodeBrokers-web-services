using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Domain.Repositories;
using VitaLink.Platform.API.Notification.Application.Errors;
using VitaLink.Platform.API.Notification.Application.Services;
using VitaLink.Platform.API.Notification.Domain.Model.Commands;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Application.Internal.EventHandlers;

/// <summary>
///     Sends the email notification of a newly raised alert to the configured family caregiver.
/// </summary>
/// <remarks>
///     A failure here is logged and never propagated, so the alert stays created even when
///     the email cannot be sent. Repeated calls for the same alert are ignored by the
///     idempotency of <see cref="INotificationCommandService"/>.
/// </remarks>
/// <param name="options">Recipient configuration.</param>
/// <param name="userRepository">IAM user repository, used to resolve the recipient.</param>
/// <param name="olderAdultRepository">ElderCare repository, used to resolve the older adult name.</param>
/// <param name="notificationCommandService">Notification command service.</param>
/// <param name="localizer">Localizer for the fallback older adult name.</param>
/// <param name="logger">Logger.</param>
public class AlertRaisedEmailNotifier(
    IOptions<AlertRecipientOptions> options,
    IUserRepository userRepository,
    IOlderAdultRepository olderAdultRepository,
    INotificationCommandService notificationCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<AlertRaisedEmailNotifier> logger)
{
    /// <summary>
    ///     Sends the email notification of an alert that was just saved.
    /// </summary>
    /// <param name="alertId">Identifier of the saved alert.</param>
    /// <param name="olderAdultId">Older adult the alert is about.</param>
    /// <param name="severity">Severity of the alert.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    public async Task NotifyAsync(Guid alertId, Guid olderAdultId, SeverityLevel severity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!options.Value.HasRecipient)
            {
                logger.LogWarning("No alert recipient is configured; alert {AlertId} was not notified", alertId);
                return;
            }

            var recipient = await userRepository.FindByEmailAsync(new Email(options.Value.Email!), cancellationToken);
            if (recipient is null)
            {
                logger.LogWarning("The configured alert recipient is not registered; alert {AlertId} was not notified",
                    alertId);
                return;
            }

            var olderAdult = await olderAdultRepository.FindByIdAsync(new OlderAdultId(olderAdultId), cancellationToken);
            var olderAdultName = olderAdult?.FullName.Display() ?? localizer["NotificationOlderAdultFallback"].Value;

            var command = new SendAlertNotificationCommand(
                alertId,
                recipient.Id.Value,
                recipient.Email.Value,
                olderAdultName,
                severity.ToString().ToUpperInvariant());

            var result = await notificationCommandService.Handle(command, cancellationToken);
            if (result is Result<NotificationAggregate, SendAlertNotificationError>.Failure failure)
                logger.LogWarning("Alert {AlertId} was not notified to recipient {RecipientId}: {Error}",
                    alertId, recipient.Id.Value, failure.Error);
            else
                logger.LogInformation("Alert {AlertId} was notified to recipient {RecipientId}",
                    alertId, recipient.Id.Value);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "The email notification of alert {AlertId} could not be sent", alertId);
        }
    }
}
