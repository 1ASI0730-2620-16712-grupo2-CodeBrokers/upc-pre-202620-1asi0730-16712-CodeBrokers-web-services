using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST;

/// <summary>
///     Exposes the clinics used by the registration forms.
/// </summary>
[ApiController]
[Route("clinics")]
[Produces(MediaTypeNames.Application.Json)]
public class ClinicsController(IClinicQueryService clinicQueryService) : ControllerBase
{
    /// <summary>
    ///     Lists the clinics ordered by name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClinicResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClinics(CancellationToken cancellationToken)
    {
        var clinics = await clinicQueryService.Handle(new GetClinicsQuery(), cancellationToken);

        return Ok(clinics.Select(ClinicResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
