using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;
using VitaLink.Platform.API.ElderCare.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST;

/// <summary>
///     Exposes the care providers used by the registration forms.
/// </summary>
[ApiController]
[Route("care-providers")]
[Produces(MediaTypeNames.Application.Json)]
public class CareProvidersController(
    ICareProviderQueryService careProviderQueryService,
    IStringLocalizer<ElderCareMessages> localizer) : ControllerBase
{
    /// <summary>
    ///     Lists the active care providers. Use <c>clinicId</c> for the providers of one clinic
    ///     or <c>independent=true</c> for those without a clinic.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CareProviderResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCareProviders(
        [FromQuery] Guid? clinicId,
        [FromQuery] bool independent,
        CancellationToken cancellationToken)
    {
        if (clinicId is not null && independent)
        {
            ModelState.AddModelError(
                nameof(independent),
                localizer["ElderCareClinicOrIndependent"].Value);

            return ValidationProblem(ModelState);
        }

        var providers = await careProviderQueryService.Handle(
            new GetCareProvidersQuery(clinicId, independent),
            cancellationToken);

        return Ok(providers.Select(CareProviderResourceFromSummaryAssembler.ToResourceFromSummary));
    }
}
