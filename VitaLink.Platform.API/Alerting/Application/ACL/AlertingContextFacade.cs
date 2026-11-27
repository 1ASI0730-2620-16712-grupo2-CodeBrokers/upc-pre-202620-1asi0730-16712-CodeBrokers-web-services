using VitaLink.Platform.API.Alerting.Application.CommandServices;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Interfaces.ACL;
using VitaLink.Platform.API.Shared.Application.Model;
using VitaLink.Platform.API.Alerting.Application.Results;
using VitaLink.Platform.API.Alerting.Domain.Model.Errors;

namespace VitaLink.Platform.API.Alerting.Application.ACL;

/// <summary>
///     Evaluates saved health records for other bounded contexts.
/// </summary>
public class AlertingContextFacade(
    IAlertCommandService alertCommandService,
    ILogger<AlertingContextFacade> logger) : IAlertingContextFacade
{
    public async Task<int> GenerateAlertsAsync(
        IEnumerable<Guid> healthRecordIds,
        CancellationToken cancellationToken = default)
    {
        var created = 0;

        foreach (var healthRecordId in healthRecordIds)
        {
            try
            {
                var result = await alertCommandService.Handle(
                    new GenerateAlertFromHealthRecordCommand(healthRecordId),
                    cancellationToken);

                if (result is Result<AlertEvaluationResult, CreateAlertError>.Success { Value.Created: true })
                    created++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogWarning(exception,
                    "The health record {HealthRecordId} could not be evaluated for alerts", healthRecordId);
            }
        }

        return created;
    }
}
