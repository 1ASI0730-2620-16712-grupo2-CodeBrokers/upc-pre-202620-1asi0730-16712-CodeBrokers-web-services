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
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Queries;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST;

[ApiController]
[Route("older-adults/{olderAdultId:guid}/health-records")]
[Produces(MediaTypeNames.Application.Json)]
public class HealthRecordsController(
    IHealthRecordCommandService healthRecordCommandService,
    IHealthRecordQueryService healthRecordQueryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateHealthRecord(
        [FromRoute] Guid olderAdultId,
        [FromBody] CreateHealthRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateHealthRecordCommandFromResourceAssembler.ToCommandFromResource(olderAdultId, resource);
        var result = await healthRecordCommandService.Handle(command, cancellationToken);

        return result switch
        {
            Result<HealthRecord, CreateHealthRecordError>.Success success =>
                StatusCode(StatusCodes.Status201Created, HealthRecordResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<HealthRecord, CreateHealthRecordError>.Failure failure => failure.Error switch
            {
                CreateHealthRecordError.InvalidReference => Problem(statusCode: 400, title: localizer["HealthRecordInvalidReference"].Value),
                CreateHealthRecordError.InvalidRecordType => Problem(statusCode: 400, title: localizer["HealthRecordInvalidRecordType"].Value),
                CreateHealthRecordError.ValueRequired => Problem(statusCode: 400, title: localizer["HealthRecordValueRequired"].Value),
                CreateHealthRecordError.ValueOutOfRange => Problem(statusCode: 400, title: localizer["HealthRecordValueOutOfRange"].Value),
                CreateHealthRecordError.InvalidUnit => Problem(statusCode: 400, title: localizer["HealthRecordInvalidUnit"].Value),
                CreateHealthRecordError.InvalidNote => Problem(statusCode: 400, title: localizer["HealthRecordInvalidNote"].Value),
                CreateHealthRecordError.EmptyRecord => Problem(statusCode: 400, title: localizer["HealthRecordEmpty"].Value),
                _ => Problem(statusCode: 500, title: localizer["UnexpectedServerError"].Value, detail: localizer["UnexpectedErrorProcessingRequest"].Value)
            },

            _ => Problem(statusCode: 500, title: localizer["UnexpectedServerError"].Value, detail: localizer["UnexpectedErrorProcessingRequest"].Value)
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetHealthRecords(
        [FromRoute] Guid olderAdultId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] string? type,
        CancellationToken cancellationToken)
    {
        var query = new GetHealthRecordsByOlderAdultQuery(olderAdultId, from, to, type);
        var result = await healthRecordQueryService.Handle(query, cancellationToken);

        return result switch
        {
            Result<IEnumerable<HealthRecord>, GetHealthRecordsError>.Success success =>
                Ok(success.Value.Select(HealthRecordResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<HealthRecord>, GetHealthRecordsError>.Failure failure => failure.Error switch
            {
                GetHealthRecordsError.InvalidDateRange => Problem(statusCode: 400, title: localizer["HealthRecordInvalidDateRange"].Value),
                _ => Problem(statusCode: 500, title: localizer["UnexpectedServerError"].Value, detail: localizer["UnexpectedErrorProcessingRequest"].Value)
            },

            _ => Problem(statusCode: 500, title: localizer["UnexpectedServerError"].Value, detail: localizer["UnexpectedErrorProcessingRequest"].Value)
        };
    }
}