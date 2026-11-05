
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.ElderCare.Application.Errors;
using VitaLink.ElderCare.Application.Services;
using VitaLink.ElderCare.Interfaces.REST.Assemblers;
using VitaLink.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.ElderCare.Interfaces.REST;

/// <summary>
///     Exposes operations for managing older adults.
/// </summary>
[ApiController]
[Route("older-adults")]
[Produces(MediaTypeNames.Application.Json)]
public class OlderAdultsController(
    IOlderAdultCommandService commandService) : ControllerBase
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
                        title: "Older adult not found."),

                LinkCareProviderError.CareProviderNotFound =>
                    Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Care provider not found."),

                LinkCareProviderError.CareProviderInactive =>
                    Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: "The care provider is inactive."),

                LinkCareProviderError.AlreadyAssigned =>
                    Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: "The older adult already has a care provider."),

                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred.")
            });
    }
}
