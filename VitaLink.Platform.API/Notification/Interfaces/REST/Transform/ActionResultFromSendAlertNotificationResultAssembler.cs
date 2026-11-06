using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Notification.Application.Errors;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Application.Patterns;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from the result of sending an alert notification.
/// </summary>
public static class ActionResultFromSendAlertNotificationResultAssembler
{
    /// <summary>
    ///     Translates the application result into a <c>201</c> response or a localized <c>ProblemDetails</c>.
    /// </summary>
    /// <param name="result">The result returned by the command service.</param>
    /// <param name="controller">The controller that produces the response.</param>
    /// <param name="localizer">Localizer for the response titles.</param>
    /// <param name="getNotificationByIdActionName">Action used to build the location header.</param>
    /// <returns>The HTTP action result.</returns>
    public static ActionResult ToActionResultFromSendAlertNotificationResult(
        Result<NotificationAggregate, SendAlertNotificationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer,
        string getNotificationByIdActionName) =>
        result switch
        {
            Result<NotificationAggregate, SendAlertNotificationError>.Success success =>
                controller.CreatedAtAction(getNotificationByIdActionName, new { id = success.Value.Id.Value },
                    NotificationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<NotificationAggregate, SendAlertNotificationError>.Failure
                { Error: SendAlertNotificationError.AlreadyNotified } =>
                controller.Problem(statusCode: StatusCodes.Status409Conflict,
                    title: localizer["NotificationAlreadySent"].Value),

            Result<NotificationAggregate, SendAlertNotificationError>.Failure
                { Error: SendAlertNotificationError.MaxAttemptsReached } =>
                controller.Problem(statusCode: StatusCodes.Status409Conflict,
                    title: localizer["NotificationMaxAttemptsReached"].Value),

            Result<NotificationAggregate, SendAlertNotificationError>.Failure
                { Error: SendAlertNotificationError.DeliveryFailed } =>
                controller.Problem(statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: localizer["NotificationDeliveryFailed"].Value),

            _ => controller.Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value)
        };
}