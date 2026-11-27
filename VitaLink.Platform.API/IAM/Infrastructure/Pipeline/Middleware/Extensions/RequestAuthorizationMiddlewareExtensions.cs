using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Components;

namespace VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Registers <see cref="RequestAuthorizationMiddleware" /> in the ASP.NET Core pipeline.
/// </summary>
public static class RequestAuthorizationMiddlewareExtensions
{
    /// <summary>
    ///     Adds the request authorization middleware. Call it after CORS and before the controllers.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The same builder.</returns>
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RequestAuthorizationMiddleware>();
}
