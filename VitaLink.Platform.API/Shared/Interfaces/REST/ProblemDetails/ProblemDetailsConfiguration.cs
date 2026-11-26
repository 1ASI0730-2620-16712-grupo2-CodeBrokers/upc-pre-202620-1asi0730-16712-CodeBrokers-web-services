using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Shared.Resources;

namespace VitaLink.Platform.API.Shared.Interfaces.REST.ProblemDetails;

/// <summary>
///     Customizes the RFC 7807 responses that ASP.NET Core builds automatically.
/// </summary>
/// <remarks>
///     It is registered in <c>Program.cs</c> with <c>AddProblemDetails</c>. The response for an unexpected
///     exception is built by <c>GlobalExceptionHandler</c>; this class only covers validation errors.
/// </remarks>
public static class ProblemDetailsConfiguration
{
    /// <summary>
    ///     Gives validation errors (automatic 400 and <c>ValidationProblem()</c>) a localized title.
    /// </summary>
    /// <param name="context">The context of the response being written.</param>
    public static void Customize(ProblemDetailsContext context)
    {
        if (context.ProblemDetails is not HttpValidationProblemDetails) return;

        var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        context.ProblemDetails.Title = localizer["InvalidModelStateTitle"].Value;
    }
}
