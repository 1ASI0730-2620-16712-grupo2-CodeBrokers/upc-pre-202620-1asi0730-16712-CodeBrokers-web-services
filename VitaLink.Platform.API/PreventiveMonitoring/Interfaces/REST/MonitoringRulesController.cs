using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST;

/// <summary>
///     Controller for managing monitoring rules.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MonitoringRulesController(
    IMonitoringRuleCommandService commandService,
    IMonitoringRuleQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    ///     Creates an active monitoring rule for an older adult and record type.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MonitoringRuleResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMonitoringRule([FromBody] CreateMonitoringRuleResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateMonitoringRuleCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await commandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<MonitoringRule, CreateMonitoringRuleError>.Success success =>
                StatusCode(StatusCodes.Status201Created,
                    MonitoringRuleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<MonitoringRule, CreateMonitoringRuleError>.Failure { Error: CreateMonitoringRuleError.InvalidReference } =>
                Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["MonitoringRuleInvalidReference"].Value),
            Result<MonitoringRule, CreateMonitoringRuleError>.Failure { Error: CreateMonitoringRuleError.InvalidRecordType } =>
                Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["MonitoringRuleInvalidRecordType"].Value),
            Result<MonitoringRule, CreateMonitoringRuleError>.Failure { Error: CreateMonitoringRuleError.InvalidThreshold } =>
                Problem(statusCode: StatusCodes.Status400BadRequest,
                    title: localizer["MonitoringRuleInvalidThreshold"].Value),
            Result<MonitoringRule, CreateMonitoringRuleError>.Failure { Error: CreateMonitoringRuleError.DuplicateActiveRule } =>
                Problem(statusCode: StatusCodes.Status409Conflict,
                    title: localizer["MonitoringRuleDuplicateActive"].Value),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value)
        };
    }

    /// <summary>
    ///     Lists the monitoring rules of an older adult.
    /// </summary>
    [HttpGet("older-adult/{olderAdultId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<MonitoringRuleResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesByOlderAdult(Guid olderAdultId, CancellationToken cancellationToken)
    {
        var rules = await queryService.Handle(olderAdultId, cancellationToken);
        var resources = rules.Select(MonitoringRuleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}