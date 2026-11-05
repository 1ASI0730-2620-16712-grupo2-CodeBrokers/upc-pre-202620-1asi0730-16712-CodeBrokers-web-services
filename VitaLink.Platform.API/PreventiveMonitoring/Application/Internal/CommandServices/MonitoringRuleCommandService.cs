using System;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;

/// <summary>
///     Command service implementation for monitoring rules.
/// </summary>
public class MonitoringRuleCommandService : IMonitoringRuleCommandService
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MonitoringRuleCommandService(
        IMonitoringRuleRepository monitoringRuleRepository,
        IUnitOfWork unitOfWork)
    {
        _monitoringRuleRepository = monitoringRuleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MonitoringRule?> Handle(CreateMonitoringRuleCommand command)
    {
        var rule = new MonitoringRule(command);
        await _monitoringRuleRepository.AddAsync(rule);
        await _unitOfWork.CompleteAsync();
        return rule;
    }
}