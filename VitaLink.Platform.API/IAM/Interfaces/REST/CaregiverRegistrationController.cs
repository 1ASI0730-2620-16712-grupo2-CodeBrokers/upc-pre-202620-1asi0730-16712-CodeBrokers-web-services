using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.IAM.Application.CommandServices;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;
using VitaLink.Platform.API.IAM.Interfaces.REST.Transform;
using VitaLink.Platform.API.IAM.Resources;
using VitaLink.Platform.API.Shared.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST;

/// <summary>
///     Registration of a family caregiver together with the patient and the first measurements.
/// </summary>
[ApiController]
[Route("authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class CaregiverRegistrationController(
    ICaregiverRegistrationCommandService registrationCommandService,
    IStringLocalizer<IamMessages> localizer,
    IStringLocalizer<CommonMessages> commonLocalizer) : ControllerBase
{
    /// <summary>
    ///     Creates the caregiver account, the patient and the first measurements in a single transaction.
    /// </summary>
    /// <remarks>
    ///     Everything is validated first; if any field is not valid (400) or the email is taken (409) nothing
    ///     is saved. After saving, the measurements are evaluated and the alerts that are raised send their email.
    /// </remarks>
    [HttpPost("sign-up/family-caregiver")]
    [ProducesResponseType(typeof(CaregiverRegistrationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterFamilyCaregiver(
        [FromBody] RegisterFamilyCaregiverResource resource,
        CancellationToken cancellationToken)
    {
        var result = await registrationCommandService.Handle(
            RegisterFamilyCaregiverCommandFromResourceAssembler.ToCommandFromResource(resource),
            cancellationToken);

        return result.Fold<IActionResult>(
            registration => StatusCode(
                StatusCodes.Status201Created,
                RegisterFamilyCaregiverCommandFromResourceAssembler.ToResourceFromResult(registration)),
            error =>
            {
                switch (error)
                {
                    case RegisterFamilyCaregiverError.InvalidFields invalid:
                        foreach (var field in invalid.Fields)
                            ModelState.AddModelError(field.Field, FieldMessage(field));

                        return ValidationProblem(ModelState);

                    case RegisterFamilyCaregiverError.EmailAlreadyRegistered:
                        return Problem(
                            statusCode: StatusCodes.Status409Conflict,
                            title: localizer["IamEmailAlreadyRegistered"].Value);

                    default:
                        return Problem(
                            statusCode: StatusCodes.Status500InternalServerError,
                            title: commonLocalizer["UnexpectedServerError"].Value);
                }
            });
    }

    private string FieldMessage(RegistrationFieldError field)
    {
        return field.Code switch
        {
            RegistrationFieldCode.Required => localizer["IamRegistrationFieldRequired"].Value,
            RegistrationFieldCode.TooLong => localizer["IamRegistrationFieldTooLong", field.Limit ?? 0].Value,
            RegistrationFieldCode.InvalidEmail => localizer["IamInvalidEmail"].Value,
            RegistrationFieldCode.InvalidPassword => localizer["IamInvalidPassword"].Value,
            RegistrationFieldCode.InvalidBirthDate => localizer["IamRegistrationInvalidBirthDate"].Value,
            RegistrationFieldCode.OutOfRange => localizer[
                "IamRegistrationMeasurementOutOfRange",
                field.Min?.ToString("0.##") ?? "0",
                field.Max?.ToString("0.##") ?? "-"].Value,
            RegistrationFieldCode.PressureOrder => localizer["IamRegistrationPressureOrder"].Value,
            RegistrationFieldCode.AuthorizationRequired => localizer["IamRegistrationAuthorizationRequired"].Value,
            RegistrationFieldCode.CareProviderNotFound => localizer["IamRegistrationProviderNotFound"].Value,
            RegistrationFieldCode.CareProviderInactive => localizer["IamRegistrationProviderInactive"].Value,
            _ => commonLocalizer["UnexpectedServerError"].Value
        };
    }
}
