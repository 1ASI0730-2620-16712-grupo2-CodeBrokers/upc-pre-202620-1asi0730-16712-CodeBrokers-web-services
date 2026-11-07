using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Application.Results;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Alerting.Domain.Model.Queries;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST;

/// <summary>
///     Exposes alert creation, paginated searches and alert details.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AlertsController(IAlertQueryService queryService, IAlertCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Evaluates a health record and creates a pending alert when an active rule is violated.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AlertEvaluationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AlertEvaluationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertResource resource,
        CancellationToken cancellationToken)
    {
        var command = new GenerateAlertFromHealthRecordCommand(resource.HealthRecordId.GetValueOrDefault());
        var result = await commandService.Handle(command, cancellationToken);

        if (result is Result<AlertEvaluationResult, CreateAlertError>.Success success)
        {
            var evaluation = success.Value;
            var alert = evaluation.Alert is null
                ? null
                : AlertResourceFromEntityAssembler.ToResourceFromEntity(evaluation.Alert);
            var response = new AlertEvaluationResource(evaluation.HealthRecordId, alert);

            return evaluation.Created && alert is not null
                ? CreatedAtAction(nameof(GetAlert), new { id = alert.Id }, response)
                : Ok(response);
        }

        return result switch
        {
            Result<AlertEvaluationResult, CreateAlertError>.Failure { Error: CreateAlertError.InvalidReference } =>
                Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["AlertingHealthRecordIdRequired"].Value),
            Result<AlertEvaluationResult, CreateAlertError>.Failure { Error: CreateAlertError.HealthRecordNotFound } =>
                Problem(statusCode: StatusCodes.Status404NotFound,
                    title: localizer["AlertingHealthRecordNotFound"].Value),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value)
        };
    }

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
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: localizer["AlertingAlertNotFound"].Value)
            : Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(alert));
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
