using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST;

/// <summary>
///     Controller for managing monitoring rules.
/// </summary>
[ApiController]
[Route("api/v1/monitoring-rules")]
[Produces(MediaTypeNames.Application.Json)]
public class MonitoringRulesController : ControllerBase
{
    private readonly IMonitoringRuleCommandService _monitoringRuleCommandService;
    private readonly IMonitoringRuleQueryService _monitoringRuleQueryService;

    public MonitoringRulesController(
        IMonitoringRuleCommandService monitoringRuleCommandService,
        IMonitoringRuleQueryService monitoringRuleQueryService)
    {
        _monitoringRuleCommandService = monitoringRuleCommandService;
        _monitoringRuleQueryService = monitoringRuleQueryService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MonitoringRuleResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMonitoringRule([FromBody] CreateMonitoringRuleResource resource)
    {
        var command = CreateMonitoringRuleCommandFromResourceAssembler.ToCommandFromResource(resource);
        var rule = await _monitoringRuleCommandService.Handle(command);

        if (rule is null) return BadRequest();

        var response = MonitoringRuleResourceFromEntityAssembler.ToResourceFromEntity(rule);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("older-adult/{olderAdultId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<MonitoringRuleResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesByOlderAdult(Guid olderAdultId)
    {
        var rules = await _monitoringRuleQueryService.Handle(olderAdultId);
        var resources = rules.Select(MonitoringRuleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}