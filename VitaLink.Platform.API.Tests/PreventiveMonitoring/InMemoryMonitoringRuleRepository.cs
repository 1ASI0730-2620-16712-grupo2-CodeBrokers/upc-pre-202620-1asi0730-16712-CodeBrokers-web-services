using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;

namespace VitaLink.Platform.API.Tests.PreventiveMonitoring;

/// <summary>
///     In-memory monitoring rule repository used to test the domain services without a database.
/// </summary>
internal sealed class InMemoryMonitoringRuleRepository(params MonitoringRule[] rules) : IMonitoringRuleRepository
{
    private readonly List<MonitoringRule> _rules = [.. rules];

    public Task AddAsync(MonitoringRule entity, CancellationToken cancellationToken = default)
    {
        _rules.Add(entity);
        return Task.CompletedTask;
    }

    public Task<MonitoringRule?> FindByIdAsync(MonitoringRuleId id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_rules.FirstOrDefault(rule => rule.Id == id));

    public void Update(MonitoringRule entity)
    {
    }

    public void Remove(MonitoringRule entity) => _rules.Remove(entity);

    public Task<IEnumerable<MonitoringRule>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<MonitoringRule>>(_rules);

    public Task<IEnumerable<MonitoringRule>> FindByOlderAdultAsync(Guid olderAdultId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_rules.Where(rule => rule.OlderAdultId == olderAdultId));

    public Task<MonitoringRule?> FindByOlderAdultAndTypeAsync(Guid olderAdultId, string recordTypeCode,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_rules.FirstOrDefault(rule =>
            rule.OlderAdultId == olderAdultId && rule.RecordTypeCode == recordTypeCode));

    public Task<IReadOnlyList<MonitoringRule>> FindActiveByOlderAdultAndTypeAsync(Guid olderAdultId,
        string recordTypeCode, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<MonitoringRule>>(_rules
            .Where(rule => rule.IsActive && rule.OlderAdultId == olderAdultId && rule.RecordTypeCode == recordTypeCode)
            .ToList());
}
