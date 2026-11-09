using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class RecordTypeRepository(AppDbContext context) : IRecordTypeRepository
{
    public Task<RecordType?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        context.Set<RecordType>().AsNoTracking().FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
}