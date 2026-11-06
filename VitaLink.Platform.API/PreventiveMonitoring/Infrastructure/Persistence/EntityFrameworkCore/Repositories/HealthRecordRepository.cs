using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core implementation of IHealthRecordRepository.
/// </summary>
public class HealthRecordRepository : IHealthRecordRepository
{
    private readonly AppDbContext _context;

    public HealthRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(HealthRecord record)
    {
        await _context.Set<HealthRecord>().AddAsync(record);
    }

    public async Task<IEnumerable<HealthRecord>> FindByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<HealthRecord>()
            .Where(r => r.OlderAdultId == olderAdultId)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<HealthRecord?> FindLatestByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<HealthRecord>()
            .Where(r => r.OlderAdultId == olderAdultId)
            .OrderByDescending(r => r.RecordedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}