using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Queries;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;

public interface IHealthRecordQueryService
{
    Task<Result<IEnumerable<HealthRecord>, GetHealthRecordsError>> Handle(
        GetHealthRecordsByOlderAdultQuery query,
        CancellationToken cancellationToken = default);
}