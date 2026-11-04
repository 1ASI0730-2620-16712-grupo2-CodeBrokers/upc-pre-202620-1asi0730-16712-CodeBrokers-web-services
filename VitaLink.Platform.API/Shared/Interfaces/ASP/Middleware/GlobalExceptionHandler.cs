using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VitaLink.Platform.API.Shared.Interfaces.ASP.Middleware;

/// <summary>
///     Global handler for unexpected exceptions.
/// </summary>
/// <remarks>
///     Logs the exception and returns an RFC 7807 <see cref="ProblemDetails"/> response
///     with status <c>500 Internal Server Error</c>. Exception details (message, stack trace)
///     are never sent to the client. Expected business failures must not throw: command
///     services return a <c>Result</c> and controllers translate it into an HTTP response.
/// </remarks>
/// <param name="problemDetailsService">Service that writes <see cref="ProblemDetails"/> responses.</param>
/// <param name="logger">The logger.</param>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "The request could not be processed. Please try again later."
            }
        });
    }
}
