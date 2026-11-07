using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;

/// <summary>
///     Command service interface for monitoring rules.
/// </summary>
public interface IMonitoringRuleCommandService
{
    /// <summary>
    ///     Creates a monitoring rule.
    /// </summary>
    /// <param name="command">The rule to create.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created rule or an expected business error.</returns>
    Task<Result<MonitoringRule, CreateMonitoringRuleError>> Handle(
        CreateMonitoringRuleCommand command,
        CancellationToken cancellationToken = default);
}