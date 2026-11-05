using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Repository implementation for health records.
/// </summary>
public class HealthRecordRepository : BaseRepository<HealthRecord, HealthRecordId>, IHealthRecordRepository
{
    public HealthRecordRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HealthRecord>> FindByOlderAdultAsync(Guid olderAdultId, DateTimeOffset? from, DateTimeOffset? to, string? recordTypeCode)
    {
        var query = Context.Set<HealthRecord>().Where(r => r.OlderAdultId == olderAdultId);

        if (from.HasValue)
            query = query.Where(r => r.RecordedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(r => r.RecordedAt <= to.Value);

        if (!string.IsNullOrWhiteSpace(recordTypeCode))
            query = query.Where(r => r.RecordTypeCode == recordTypeCode);

        return await query.OrderByDescending(r => r.RecordedAt).ToListAsync();
    }

    public async Task<IEnumerable<HealthRecord>> FindLatestByOlderAdultAsync(Guid olderAdultId, int take)
    {
        return await Context.Set<HealthRecord>()
            .Where(r => r.OlderAdultId == olderAdultId)
            .OrderByDescending(r => r.RecordedAt)
            .Take(take)
            .ToListAsync();
    }
}