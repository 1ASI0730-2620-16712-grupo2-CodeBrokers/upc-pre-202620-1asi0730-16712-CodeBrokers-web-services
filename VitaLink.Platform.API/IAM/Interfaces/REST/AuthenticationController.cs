using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.IAM.Application.CommandServices;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;
using VitaLink.Platform.API.IAM.Interfaces.REST.Transform;
using VitaLink.Platform.API.Shared.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST;

/// <summary>
///     Account registration and sign-in.
/// </summary>
[ApiController]
[Route("authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Registers a care provider or family caregiver account.
    /// </summary>
    /// <param name="resource">Sign-up data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("sign-up")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        if (!UserRoleText.TryParse(resource.Role, out var role))
        {
            ModelState.AddModelError(nameof(resource.Role), localizer["IamInvalidRole"].Value);
            return ValidationProblem(ModelState);
        }

        var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource, role);
        var result = await userCommandService.Handle(command, cancellationToken);

        return result.Fold<IActionResult>(
            user => StatusCode(StatusCodes.Status201Created, UserResourceFromEntityAssembler.ToResourceFromEntity(user)),
            error => error switch
            {
                SignUpError.InvalidEmail => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["IamInvalidEmail"].Value),
                SignUpError.InvalidPassword => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["IamInvalidPassword"].Value),
                SignUpError.RoleNotAllowed => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["IamRoleNotAllowed"].Value),
                SignUpError.EmailAlreadyRegistered => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: localizer["IamEmailAlreadyRegistered"].Value),
                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: localizer["UnexpectedServerError"].Value)
            });
    }

    /// <summary>
    ///     Signs in and returns an access token.
    /// </summary>
    /// <param name="resource">Sign-in data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return result.Fold<IActionResult>(
            authenticated => Ok(AuthenticatedUserResourceFromResultAssembler.ToResourceFromResult(authenticated)),
            error => error switch
            {
                SignInError.InvalidCredentials => Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: localizer["IamInvalidCredentials"].Value),
                SignInError.AccountDisabled => Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: localizer["IamAccountDisabled"].Value),
                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: localizer["UnexpectedServerError"].Value)
            });
    }
}
