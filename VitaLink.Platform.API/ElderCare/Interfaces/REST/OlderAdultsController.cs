using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Resources;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.ElderCare.Application.Errors;
using VitaLink.Platform.API.ElderCare.Application.Services;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Assemblers;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST;

/// <summary>
///     Exposes operations for managing older adults.
/// </summary>
[ApiController]
[Route("older-adults")]
[Produces(MediaTypeNames.Application.Json)]
public class OlderAdultsController(
    IOlderAdultCommandService commandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Associates an older adult with an active care provider.
    /// </summary>
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
                    title: localizer["UnexpectedServerError"].Value)
            });
    }
}
