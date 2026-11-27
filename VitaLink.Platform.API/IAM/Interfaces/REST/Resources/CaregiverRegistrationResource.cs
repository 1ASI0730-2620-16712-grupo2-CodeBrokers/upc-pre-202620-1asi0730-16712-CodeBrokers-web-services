namespace VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     What the registration of a family caregiver created.
/// </summary>
/// <param name="User">The caregiver account.</param>
/// <param name="OlderAdultId">The registered patient.</param>
/// <param name="HealthRecordIds">The saved first measurements.</param>
/// <param name="AlertsCreated">Alerts raised by those measurements.</param>
public sealed record CaregiverRegistrationResource(
    UserResource User,
    Guid OlderAdultId,
    IReadOnlyList<Guid> HealthRecordIds,
    int AlertsCreated);
