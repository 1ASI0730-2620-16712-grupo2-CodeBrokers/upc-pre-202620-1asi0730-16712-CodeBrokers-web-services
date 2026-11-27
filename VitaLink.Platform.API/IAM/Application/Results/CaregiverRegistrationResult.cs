using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.IAM.Application.Results;

/// <summary>
///     What was saved when a family caregiver registered.
/// </summary>
/// <param name="User">The caregiver account.</param>
/// <param name="OlderAdultId">The registered patient.</param>
/// <param name="HealthRecordIds">The saved first measurements.</param>
/// <param name="AlertsCreated">Alerts raised by those measurements after saving.</param>
public sealed record CaregiverRegistrationResult(
    User User,
    Guid OlderAdultId,
    IReadOnlyList<Guid> HealthRecordIds,
    int AlertsCreated);
