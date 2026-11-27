using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Queries;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.QueryServices;

public interface IHealthRecordQueryService
{
    Task<Result<IEnumerable<HealthRecord>, GetHealthRecordsError>> Handle(
        GetHealthRecordsByOlderAdultQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the most recent record of each record type for an older adult.
    /// </summary>
    Task<IReadOnlyList<HealthRecord>> Handle(
        GetLatestHealthRecordsByOlderAdultQuery query,
        CancellationToken cancellationToken = default);
}
