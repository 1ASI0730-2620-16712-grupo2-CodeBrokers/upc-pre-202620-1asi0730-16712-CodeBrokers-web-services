using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.ElderCare.Resources;
using VitaLink.Platform.API.Shared.Resources;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.ElderCare.Domain.Model.Errors;
using VitaLink.Platform.API.ElderCare.Application.CommandServices;
using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST;

/// <summary>
///     Exposes operations for managing older adults.
/// </summary>
[ApiController]
[Authorize]
[Route("older-adults")]
[Produces(MediaTypeNames.Application.Json)]
public class OlderAdultsController(
    IOlderAdultCommandService commandService,
    IOlderAdultQueryService queryService,
    IStringLocalizer<ElderCareMessages> localizer,
    IStringLocalizer<CommonMessages> commonLocalizer) : ControllerBase
{
    /// <summary>
    ///     Lists the older adults. Filter by <c>search</c> (text in the full name), <c>careProviderId</c>
    ///     or <c>isMonitoringActive</c>.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OlderAdultResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOlderAdults(
        [FromQuery] string? search,
        [FromQuery] Guid? careProviderId,
        [FromQuery] bool? isMonitoringActive,
        CancellationToken cancellationToken)
    {
        var olderAdults = await queryService.Handle(
            new GetOlderAdultsQuery(search, careProviderId, isMonitoringActive),
            cancellationToken);

        return Ok(olderAdults.Select(OlderAdultResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Gets one older adult.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OlderAdultResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOlderAdultById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var olderAdult = await queryService.Handle(
            new GetOlderAdultByIdQuery(id),
            cancellationToken);

        if (olderAdult is null)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: localizer["ElderCareOlderAdultNotFound"].Value);

        return Ok(OlderAdultResourceFromEntityAssembler.ToResourceFromEntity(olderAdult));
    }

    /// <summary>
    ///     Registers an older adult. An incomplete profile is saved with the monitoring inactive;
    ///     activating the monitoring needs the address and the emergency contact.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OlderAdultResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOlderAdult(
        [FromBody] SaveOlderAdultResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(
            SaveOlderAdultCommandFromResourceAssembler.ToCreateCommand(resource),
            cancellationToken);

        return result.Fold<IActionResult>(
            olderAdult => CreatedAtAction(
                nameof(GetOlderAdultById),
                new { id = olderAdult.Id.Value },
                OlderAdultResourceFromEntityAssembler.ToResourceFromEntity(olderAdult)),
            ToErrorResponse);
    }

    /// <summary>
    ///     Updates the profile of an older adult. The body replaces every profile field; when it is not valid
    ///     the stored version is kept.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OlderAdultResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOlderAdult(
        Guid id,
        [FromBody] SaveOlderAdultResource resource,
        CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(
            SaveOlderAdultCommandFromResourceAssembler.ToUpdateCommand(id, resource),
            cancellationToken);

        return result.Fold<IActionResult>(
            olderAdult => Ok(OlderAdultResourceFromEntityAssembler.ToResourceFromEntity(olderAdult)),
            ToErrorResponse);
    }

    /// <summary>
    ///     Associates an older adult with an active care provider.
    /// </summary>
    [Authorize(UserRole.CareProvider)]
    [HttpPatch("{id:guid}/care-provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> LinkCareProvider(
        Guid id,
        [FromBody] LinkCareProviderResource resource,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            ModelState.AddModelError(
                nameof(id),
                "The older adult identifier cannot be empty.");

        if (resource is null)
        {
            ModelState.AddModelError(
                nameof(resource),
                "Request body is required.");

            return ValidationProblem(ModelState);
        }

        if (resource.CareProviderId == Guid.Empty)
            ModelState.AddModelError(
                nameof(resource.CareProviderId),
                "The care provider identifier cannot be empty.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var command = LinkCareProviderCommandFromResourceAssembler
            .ToCommandFromResource(id, resource);

        var result = await commandService.Handle(
            command,
            cancellationToken);

        return result.Fold<IActionResult>(
            olderAdult => Ok(new
            {
                id = olderAdult.Id.Value,
                assignedProviderId = olderAdult.AssignedProviderId
            }),
            error => error switch
            {
                LinkCareProviderError.OlderAdultNotFound =>
                    Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: localizer["ElderCareOlderAdultNotFound"].Value),

                LinkCareProviderError.CareProviderNotFound =>
                    Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: localizer["ElderCareProviderNotFound"].Value),

                LinkCareProviderError.CareProviderInactive =>
                    Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: localizer["ElderCareProviderInactive"].Value),

                LinkCareProviderError.AlreadyAssigned =>
                    Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: localizer["ElderCareAlreadyAssigned"].Value),

                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: commonLocalizer["UnexpectedServerError"].Value)
            });
    }

    private IActionResult ToErrorResponse(SaveOlderAdultError error)
    {
        switch (error)
        {
            case SaveOlderAdultError.InvalidFields invalid:
                foreach (var field in invalid.Fields)
                    ModelState.AddModelError(field.Field, FieldMessage(field));

                return ValidationProblem(ModelState);

            case SaveOlderAdultError.OlderAdultNotFound:
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: localizer["ElderCareOlderAdultNotFound"].Value);

            case SaveOlderAdultError.CareProviderNotFound:
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: localizer["ElderCareProviderNotFound"].Value);

            case SaveOlderAdultError.CareProviderInactive:
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: localizer["ElderCareProviderInactive"].Value);

            default:
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: commonLocalizer["UnexpectedServerError"].Value);
        }
    }

    private string FieldMessage(FieldError field)
    {
        return field.Code switch
        {
            FieldErrorCode.Required => localizer["ElderCareFieldRequired"].Value,
            FieldErrorCode.RequiredToActivate => localizer["ElderCareFieldRequiredToActivate"].Value,
            FieldErrorCode.TooLong => localizer["ElderCareFieldTooLong", field.Limit ?? 0].Value,
            FieldErrorCode.InvalidBirthDate => localizer["ElderCareInvalidBirthDate"].Value,
            FieldErrorCode.InvalidPhone => localizer["ElderCareInvalidPhone"].Value,
            _ => commonLocalizer["UnexpectedServerError"].Value
        };
    }
}
