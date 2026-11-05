using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Repository implementation for monitoring rules.
/// </summary>
public class MonitoringRuleRepository : BaseRepository<MonitoringRule, MonitoringRuleId>, IMonitoringRuleRepository
{
    public MonitoringRuleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MonitoringRule>> FindByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<MonitoringRule>()
            .Where(r => r.OlderAdultId == olderAdultId)
            .ToListAsync(cancellationToken);
    }

    public async Task<MonitoringRule?> FindByOlderAdultAndTypeAsync(Guid olderAdultId, string recordTypeCode, CancellationToken cancellationToken = default)
    {
        return await Context.Set<MonitoringRule>()
            .FirstOrDefaultAsync(r => r.OlderAdultId == olderAdultId && r.RecordTypeCode == recordTypeCode, cancellationToken);
    }
}