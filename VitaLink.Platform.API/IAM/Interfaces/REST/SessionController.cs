using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.IAM.Application.CommandServices;
using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Components;

namespace VitaLink.Platform.API.IAM.Interfaces.REST;

/// <summary>
///     Closes the session of the signed-in user.
/// </summary>
/// <param name="sessionCommandService">Session command service.</param>
[ApiController]
[Route("authentication")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
[Authorize]
public class SessionController(ISessionCommandService sessionCommandService) : ControllerBase
{
    /// <summary>
    ///     Signs out: the access token sent in the request is revoked and can no longer be used.
    /// </summary>
    /// <response code="204">The session was closed.</response>
    /// <response code="401">There is no valid session.</response>
    [HttpPost("sign-out")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult CloseSession()
    {
        var token = (ValidatedToken)HttpContext.Items[RequestAuthorizationMiddleware.TokenKey]!;
        sessionCommandService.Handle(new SignOutCommand(token.TokenId, token.ExpiresAt));
        return NoContent();
    }
}
