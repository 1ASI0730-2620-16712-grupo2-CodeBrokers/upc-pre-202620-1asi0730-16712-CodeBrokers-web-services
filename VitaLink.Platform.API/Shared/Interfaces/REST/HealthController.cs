using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.Shared.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Shared.Interfaces.REST;

/// <summary>
///     Exposes a health check endpoint to verify that the API is running.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class HealthController : ControllerBase
{
    /// <summary>
    ///     Returns the current status of the API.
    /// </summary>
    /// <returns>A <see cref="HealthResource"/> with the status and the UTC timestamp.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResource), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        return Ok(new HealthResource("Healthy", DateTime.UtcNow));
    }
}
