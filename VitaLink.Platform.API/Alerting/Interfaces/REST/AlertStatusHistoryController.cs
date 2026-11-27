using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.Alerting.Application.QueryServices;
using VitaLink.Platform.API.Alerting.Domain.Model.Queries;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Transform;
using VitaLink.Platform.API.Alerting.Resources;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST;

/// <summary>
///     Reads the status history of several alerts in one call, to avoid one request per alert (VTL-109).
/// </summary>
/// <param name="queryService">Alert query service.</param>
/// <param name="localizer">Localizer for the Alerting messages.</param>
[ApiController]
[Authorize]
[Route("alerts/status-history")]
[Produces(MediaTypeNames.Application.Json)]
public class AlertStatusHistoryController(
    IAlertQueryService queryService,
    IStringLocalizer<AlertingMessages> localizer) : ControllerBase
{
    /// <summary>
    ///     Gets the status history of the requested alerts.
    /// </summary>
    /// <param name="alertIds">Alert identifiers, repeated in the query string: <c>?alertIds=a&amp;alertIds=b</c> (1 to 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>One entry per existing alert, in the requested order; unknown identifiers are omitted.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertStatusHistoryResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStatusHistories([FromQuery] Guid[] alertIds,
        CancellationToken cancellationToken)
    {
        var ids = alertIds.Where(id => id != Guid.Empty).Distinct().ToList();
        if (ids.Count == 0 || ids.Count > GetAlertsStatusHistoryQuery.MaxAlerts)
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                title: localizer["AlertingStatusHistoryInvalidIds", GetAlertsStatusHistoryQuery.MaxAlerts].Value);

        var alerts = await queryService.Handle(new GetAlertsStatusHistoryQuery(ids), cancellationToken);
        var byId = alerts.ToDictionary(alert => alert.Id.Value);

        var resources = ids
            .Where(byId.ContainsKey)
            .Select(id => new AlertStatusHistoryResource(id, byId[id].StatusHistory
                .OrderBy(change => change.ChangedAt)
                .Select(AlertStatusChangeResourceFromEntityAssembler.ToResourceFromEntity)
                .ToList()));

        return Ok(resources);
    }
}
