using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;

public interface IRecordTypeRepository
{
    Task<RecordType?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
}