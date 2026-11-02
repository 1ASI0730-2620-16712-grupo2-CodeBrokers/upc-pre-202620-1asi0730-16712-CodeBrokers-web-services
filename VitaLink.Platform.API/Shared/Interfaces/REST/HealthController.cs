using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Shared.Interfaces.REST;

/// <summary>
///     Exposes a health check endpoint to verify that the API and its database are available.
/// </summary>
/// <param name="context">The EF Core database context used to check the database connection.</param>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class HealthController(AppDbContext context) : ControllerBase
{
    /// <summary>
    ///     Returns the current status of the API and its database connection.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     <c>200 OK</c> when the database is reachable; otherwise, <c>503 Service Unavailable</c>.
    ///     Both responses contain a <see cref="HealthResource"/>.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthResource), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var databaseAvailable = await context.Database.CanConnectAsync(cancellationToken);

        if (!databaseAvailable)
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new HealthResource("Unhealthy", "Down", DateTime.UtcNow));

        return Ok(new HealthResource("Healthy", "Up", DateTime.UtcNow));
    }
}
