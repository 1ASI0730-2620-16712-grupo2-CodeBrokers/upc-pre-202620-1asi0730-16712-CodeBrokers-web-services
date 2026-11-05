
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Transform;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST;

/// <summary>
///     Exposes alert status change operations.
/// </summary>
[ApiController]
[Route("alerts")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertStatusController(
    IAlertStatusCommandService commandService,
    IAlertQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
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
            ModelState.AddModelError(
                nameof(id),
                "The alert identifier cannot be empty.");
        if (resource is null)
        {
            ModelState.AddModelError(
                nameof(resource),
                "Request body is required.");

            return ValidationProblem(ModelState);
        }

        if (resource.ChangedBy == Guid.Empty)
            ModelState.AddModelError(
                nameof(resource.ChangedBy),
                "ChangedBy cannot be empty.");

        if (resource.Version == 0)
            ModelState.AddModelError(
                nameof(resource.Version),
                "Version must be greater than zero.");

        if (string.IsNullOrWhiteSpace(resource.Status))
            ModelState.AddModelError(
                nameof(resource.Status),
                localizer["AlertingStatusRequired"].Value);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var command = UpdateAlertStatusCommandFromResourceAssembler
            .ToCommandFromResource(id, resource);

        var result = await commandService.Handle(
            command,
            cancellationToken);

        return result.Fold<IActionResult>(
            alert => Ok(
                AlertStatusResourceFromEntityAssembler.ToResourceFromEntity(alert)),
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
    /// <summary>
    ///     Retrieves the status change history of an alert.
    /// </summary>
    [HttpGet("{id:guid}/status/history")]
    [ProducesResponseType(
        typeof(IEnumerable<AlertStatusChangeResource>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlertStatusHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            ModelState.AddModelError(
                nameof(id),
                "The alert identifier cannot be empty.");

            return ValidationProblem(ModelState);
        }

        var alert = await queryService.Handle(
            new AlertId(id),
            cancellationToken);

        if (alert is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: localizer["ResourceNotFound"].Value);
        }

        var history = alert.StatusHistory
            .OrderBy(change => change.ChangedAt)
            .Select(AlertStatusChangeResourceFromEntityAssembler.ToResourceFromEntity)
            .ToList();

        return Ok(history);
    }
}
