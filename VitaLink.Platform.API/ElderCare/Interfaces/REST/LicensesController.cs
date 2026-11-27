using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST;

/// <summary>
///     Validates professional licenses (simulated verification) for the "Validate license" button.
/// </summary>
[ApiController]
[Route("licenses")]
[Produces(MediaTypeNames.Application.Json)]
public class LicensesController(ILicenseQueryService licenseQueryService) : ControllerBase
{
    /// <summary>
    ///     Checks whether a license number can be used to register as a care provider.
    /// </summary>
    /// <remarks>
    ///     The verification is simulated: the format (4 to 10 digits) is checked and the number is looked up
    ///     in a registry of test data. A license that is not valid is still a 200 response with
    ///     <c>isValid = false</c> and the reason.
    /// </remarks>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(LicenseValidationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateLicense(
        [FromBody] ValidateLicenseResource resource,
        CancellationToken cancellationToken)
    {
        var result = await licenseQueryService.Handle(
            new ValidateLicenseQuery(resource.LicenseNumber),
            cancellationToken);

        return Ok(LicenseValidationResourceFromResultAssembler.ToResourceFromResult(
            resource.LicenseNumber,
            result));
    }
}
