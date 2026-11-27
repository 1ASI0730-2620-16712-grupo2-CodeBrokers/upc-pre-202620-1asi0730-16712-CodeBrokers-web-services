using VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;
using VitaLink.Platform.API.IAM.Application.QueryServices;
using VitaLink.Platform.API.IAM.Domain.Model.Queries;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
///     Reads the Bearer token of the request and, when it is valid, sets the signed-in user for the rest of the
///     pipeline (<see cref="AuthorizeAttribute" /> and the controllers).
/// </summary>
/// <remarks>
///     The token must be valid (signature, issuer, audience, expiration and not revoked) and the account must be
///     enabled. Otherwise no user is set and the protected endpoints answer 401. This middleware never blocks a
///     request by itself: the attributes decide.
/// </remarks>
/// <param name="next">Next middleware of the pipeline.</param>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /// <summary>Key of the signed-in <c>User</c> in <c>HttpContext.Items</c>.</summary>
    public const string UserKey = "IAM.User";

    /// <summary>Key of the validated token data (<c>ValidatedToken</c>) in <c>HttpContext.Items</c>.</summary>
    public const string TokenKey = "IAM.Token";

    /// <summary>
    ///     Validates the token of the request and loads its user.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="tokenService">Token service.</param>
    /// <param name="userQueryService">User query service.</param>
    public async Task InvokeAsync(HttpContext context, ITokenService tokenService, IUserQueryService userQueryService)
    {
        var allowAnonymous = context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null;
        var header = context.Request.Headers.Authorization.ToString();

        if (!allowAnonymous && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = await tokenService.ValidateAsync(header["Bearer ".Length..].Trim());
            if (token is not null)
            {
                var user = await userQueryService.Handle(new GetUserByIdQuery(token.UserId), context.RequestAborted);
                if (user is not null && user.IsEnabled())
                {
                    context.Items[UserKey] = user;
                    context.Items[TokenKey] = token;
                }
            }
        }

        await next(context);
    }
}
