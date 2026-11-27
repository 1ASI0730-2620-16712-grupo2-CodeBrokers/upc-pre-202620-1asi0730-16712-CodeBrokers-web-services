using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Components;
using VitaLink.Platform.API.IAM.Resources;

namespace VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Requires a signed-in user and, optionally, one of the given roles.
/// </summary>
/// <remarks>
///     The user is set by <see cref="RequestAuthorizationMiddleware" /> from the Bearer token.
///     No user returns 401; a user with another role returns 403. On a controller and on one of its actions
///     both attributes apply, so the action can narrow the roles of the controller.
/// </remarks>
/// <param name="roles">Roles allowed; empty means any signed-in user.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeAttribute(params UserRole[] roles) : Attribute, IAuthorizationFilter
{
    /// <summary>
    ///     Roles allowed by this attribute; empty means any signed-in user.
    /// </summary>
    public IReadOnlyList<UserRole> Roles { get; } = roles;

    /// <summary>
    ///     Checks the signed-in user and its role.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;

        var localizer = context.HttpContext.RequestServices?.GetService(typeof(IStringLocalizer<IamMessages>))
            as IStringLocalizer<IamMessages>;

        if (context.HttpContext.Items[RequestAuthorizationMiddleware.UserKey] is not User user)
        {
            context.Result = Problem(StatusCodes.Status401Unauthorized,
                localizer?["IamUnauthorized"].Value ?? "Sign in to continue.");
            return;
        }

        if (Roles.Count > 0 && !Roles.Contains(user.Role))
            context.Result = Problem(StatusCodes.Status403Forbidden,
                localizer?["IamForbidden"].Value ?? "Your role does not have access to this resource.");
    }

    private static ObjectResult Problem(int status, string title) =>
        new(new ProblemDetails { Status = status, Title = title })
        {
            StatusCode = status,
            ContentTypes = { "application/problem+json" }
        };
}
