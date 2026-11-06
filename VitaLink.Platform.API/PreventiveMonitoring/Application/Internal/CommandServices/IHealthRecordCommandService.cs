using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;

public interface IHealthRecordCommandService
{
    Task<Result<HealthRecord, CreateHealthRecordError>> Handle(
        CreateHealthRecordCommand command,
        CancellationToken cancellationToken = default);
}