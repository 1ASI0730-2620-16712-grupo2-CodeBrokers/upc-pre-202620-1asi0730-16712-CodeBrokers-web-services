using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Queries;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;

public class HealthRecordQueryService(IHealthRecordRepository healthRecordRepository) : IHealthRecordQueryService
{
    private readonly IHealthRecordRepository _healthRecordRepository = healthRecordRepository;

    public async Task<Result<IEnumerable<HealthRecord>, GetHealthRecordsError>> Handle(
        GetHealthRecordsByOlderAdultQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.From.HasValue && query.To.HasValue && query.From > query.To)
        {
            return new Result<IEnumerable<HealthRecord>, GetHealthRecordsError>.Failure(GetHealthRecordsError.InvalidDateRange);
        }

        var type = string.IsNullOrWhiteSpace(query.RecordTypeCode)
            ? null
            : query.RecordTypeCode.Trim().ToUpperInvariant();

        var records = await _healthRecordRepository.FindByOlderAdultAsync(
            query.OlderAdultId,
            query.From,
            query.To,
            type,
            cancellationToken);

        return new Result<IEnumerable<HealthRecord>, GetHealthRecordsError>.Success(records);
    }
}