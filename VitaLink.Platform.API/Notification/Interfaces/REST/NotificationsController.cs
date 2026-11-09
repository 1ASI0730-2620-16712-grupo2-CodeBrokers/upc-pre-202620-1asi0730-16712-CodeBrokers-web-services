using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Notification.Application.Services;
using VitaLink.Platform.API.Notification.Domain.Model.Queries;
using VitaLink.Platform.API.Notification.Interfaces.REST.Resources;
using VitaLink.Platform.API.Notification.Interfaces.REST.Transform;
using VitaLink.Platform.API.Resources;

namespace VitaLink.Platform.API.Notification.Interfaces.REST;

/// <summary>
///     Sends alert notifications and exposes their delivery status.
/// </summary>
/// <param name="notificationCommandService">Notification command service.</param>
/// <param name="notificationQueryService">Notification query service.</param>
/// <param name="localizer">Localizer for the response titles.</param>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Notifications")]
public class NotificationsController(
    INotificationCommandService notificationCommandService,
    INotificationQueryService notificationQueryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Sends the email notification of an alert to a recipient.
    /// </summary>
    /// <param name="resource">Alert and recipient data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> SendAlertNotification(
        [FromBody] SendAlertNotificationResource resource, CancellationToken cancellationToken)
    {
        var command = SendAlertNotificationCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await notificationCommandService.Handle(command, cancellationToken);
        return ActionResultFromSendAlertNotificationResultAssembler.ToActionResultFromSendAlertNotificationResult(
            result, this, localizer, nameof(GetNotificationById));
    }

    /// <summary>
    ///     Gets a notification by its identifier.
    /// </summary>
    /// <param name="id">Notification identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NotificationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetNotificationById(Guid id, CancellationToken cancellationToken)
    {
        var notification = await notificationQueryService.Handle(new GetNotificationByIdQuery(id), cancellationToken);
        if (notification is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: localizer["NotificationNotFound"].Value);

        return Ok(NotificationResourceFromEntityAssembler.ToResourceFromEntity(notification));
    }

    /// <summary>
    ///     Lists the notifications addressed to a recipient, most recent first.
    /// </summary>
    /// <param name="recipientId">Recipient user identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetNotificationsByRecipientId(
        [FromQuery] [BindRequired] Guid recipientId, CancellationToken cancellationToken)
    {
        var notifications = await notificationQueryService.Handle(
            new GetNotificationsByRecipientIdQuery(recipientId), cancellationToken);
        return Ok(notifications.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity));
    }
}