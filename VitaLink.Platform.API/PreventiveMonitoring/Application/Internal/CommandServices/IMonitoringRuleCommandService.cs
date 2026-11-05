using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;

/// <summary>
///     Command service interface for monitoring rules.
/// </summary>
public interface IMonitoringRuleCommandService
{
    Task<MonitoringRule?> Handle(CreateMonitoringRuleCommand command);
}