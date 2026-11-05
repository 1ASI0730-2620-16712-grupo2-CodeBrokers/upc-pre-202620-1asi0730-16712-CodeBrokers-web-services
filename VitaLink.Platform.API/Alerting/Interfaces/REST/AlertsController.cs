using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.Alerting.Domain.Model.Queries;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Transform;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST;

/// <summary>
///     Exposes paginated alert searches and alert details.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertsController(
    IAlertQueryService queryService,
    IAlertStatusCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Lists alerts, optionally filtered by status and patient.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(AlertPageResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAlerts([FromQuery] string? status = null,
        [FromQuery] Guid? olderAdultId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        AlertStatus? parsedStatus = null;
        if (status is not null)
        {
            parsedStatus = status.ToUpperInvariant() switch
            {
                "PENDING" => AlertStatus.Pending,
                "IN_REVIEW" => AlertStatus.InReview,
                "ATTENDED" => AlertStatus.Attended,
                "CLOSED" => AlertStatus.Closed,
                _ => null
            };
            if (parsedStatus is null)
                ModelState.AddModelError(nameof(status), "Use PENDING, IN_REVIEW, ATTENDED or CLOSED.");
        }
        return await FindPage(parsedStatus, olderAdultId, page, pageSize, cancellationToken);
    }

    /// <summary>
    ///     Lists pending alerts, optionally filtered by patient.
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(AlertPageResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetPendingAlerts([FromQuery] Guid? olderAdultId = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        FindPage(AlertStatus.Pending, olderAdultId, page, pageSize, cancellationToken);

    /// <summary>
    ///     Finds an alert by identifier, including closed alerts.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlert(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(id), "The alert identifier cannot be empty.");
            return ValidationProblem(ModelState);
        }
        var alert = await queryService.Handle(new AlertId(id), cancellationToken);
        return alert is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Alert not found.")
            : Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(alert));
    }
    
    
    /// <summary>
    ///     Updates an alert status with optimistic concurrency control.
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(AlertStatusResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAlertStatus(
        Guid id,
        [FromBody] UpdateAlertStatusResource resource,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            ModelState.AddModelError(nameof(id), "The alert identifier cannot be empty.");

        if (resource is null)
        {
            ModelState.AddModelError(nameof(resource), "Request body is required.");
            return ValidationProblem(ModelState);
        }

        if (resource.ChangedBy == Guid.Empty)
            ModelState.AddModelError(nameof(resource.ChangedBy), "ChangedBy cannot be empty.");

        if (resource.Version == 0)
            ModelState.AddModelError(nameof(resource.Version), "Version must be greater than zero.");

        if (string.IsNullOrWhiteSpace(resource.Status))
            ModelState.AddModelError(
                nameof(resource.Status),
                localizer["AlertingStatusRequired"].Value);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var command = UpdateAlertStatusCommandFromResourceAssembler
            .ToCommandFromResource(id, resource);

        var result = await commandService.Handle(command, cancellationToken);

        
        return result.Fold<IActionResult>(
            alert => Ok(AlertStatusResourceFromEntityAssembler.ToResourceFromEntity(alert)),
            error => error switch
            {
                UpdateAlertStatusError.AlertNotFound =>
                    Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: localizer["ResourceNotFound"].Value),

                UpdateAlertStatusError.InvalidTransition =>
                    Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: localizer["AlertingInvalidStatusTransition"].Value),

                UpdateAlertStatusError.ConcurrencyConflict =>
                    Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: localizer["ConcurrencyConflict"].Value),

                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: localizer["UnexpectedServerError"].Value)
            });

    }


    private async Task<IActionResult> FindPage(AlertStatus? status, Guid? olderAdultId,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        if (olderAdultId == Guid.Empty)
            ModelState.AddModelError(nameof(olderAdultId), "The patient identifier cannot be empty.");
        if (page < 1)
            ModelState.AddModelError(nameof(page), "Page must be at least 1.");
        if (pageSize is < 1 or > 100)
            ModelState.AddModelError(nameof(pageSize), "Page size must be between 1 and 100.");
        if (page > 0 && pageSize is >= 1 and <= 100 && (long)(page - 1) * pageSize > int.MaxValue)
            ModelState.AddModelError(nameof(page), "The requested page is too large.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await queryService.Handle(new GetAlertsQuery(status, olderAdultId, page, pageSize), cancellationToken);
        return Ok(new AlertPageResource(result.Items.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity).ToList(),
            result.TotalItems, result.Page, result.PageSize, (int)Math.Ceiling((double)result.TotalItems / result.PageSize)));
    }
}
