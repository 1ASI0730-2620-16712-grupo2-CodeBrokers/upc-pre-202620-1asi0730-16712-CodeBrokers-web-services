using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST;

/// <summary>Registers and retrieves the observation for an alert.</summary>
[ApiController]
[Route("alerts/{alertId:guid}/observation")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertObservationsController(IAlertObservationCommandService commandService,
    IAlertObservationQueryService queryService, IAlertQueryService alerts,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AlertObservationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateObservation(Guid alertId, CreateAlertObservationResource resource,
        CancellationToken cancellationToken)
    {
        if (alertId == Guid.Empty || resource.CreatedBy.GetValueOrDefault() == Guid.Empty)
        {
            var field = alertId == Guid.Empty ? nameof(alertId) : "createdBy";
            ModelState.AddModelError(field, localizer["AlertingObservationInvalidReference"].Value);
            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(resource.Text))
            return NoContent();

        var command = CreateAlertObservationCommandFromResourceAssembler.ToCommandFromResource(alertId, resource);
        var result = await commandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<AlertObservation, CreateAlertObservationError>.Success success =>
                CreatedAtAction(nameof(GetObservation), new { alertId },
                    AlertObservationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<AlertObservation, CreateAlertObservationError>.Failure failure => failure.Error switch
            {
                CreateAlertObservationError.AlertNotFound =>
                    Problem(statusCode: StatusCodes.Status404NotFound, title: localizer["AlertingAlertNotFound"].Value),
                CreateAlertObservationError.AlertNotAttended =>
                    Problem(statusCode: StatusCodes.Status409Conflict, title: localizer["AlertingObservationRequiresAttended"].Value),
                CreateAlertObservationError.ObservationAlreadyExists =>
                    Problem(statusCode: StatusCodes.Status409Conflict, title: localizer["AlertingObservationAlreadyExists"].Value),
                CreateAlertObservationError.TextTooLong =>
                    Problem(statusCode: StatusCodes.Status400BadRequest,
                        title: localizer["AlertingObservationTooLong", AlertObservation.MaxLength].Value),
                CreateAlertObservationError.InvalidReference =>
                    Problem(statusCode: StatusCodes.Status400BadRequest, title: localizer["AlertingObservationInvalidReference"].Value),
                _ => UnexpectedError()
            },
            _ => UnexpectedError()
        };
    }

    [HttpGet]
    [ProducesResponseType(typeof(AlertObservationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetObservation(Guid alertId, CancellationToken cancellationToken)
    {
        if (alertId == Guid.Empty || await alerts.Handle(new AlertId(alertId), cancellationToken) is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: localizer["AlertingAlertNotFound"].Value);

        var observation = await queryService.Handle(alertId, cancellationToken);
        return observation is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: localizer["AlertingObservationNotFound"].Value)
            : Ok(AlertObservationResourceFromEntityAssembler.ToResourceFromEntity(observation));
    }

    private ObjectResult UnexpectedError() =>
        Problem(statusCode: StatusCodes.Status500InternalServerError,
            title: localizer["UnexpectedServerError"].Value,
            detail: localizer["UnexpectedErrorProcessingRequest"].Value);
}
