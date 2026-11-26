using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.CommandServices;

public interface IHealthRecordCommandService
{
    Task<Result<HealthRecord, CreateHealthRecordError>> Handle(
        CreateHealthRecordCommand command,
        CancellationToken cancellationToken = default);
}