namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     An older adult as returned by the API.
/// </summary>
public sealed record OlderAdultResource(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    int Age,
    string? Street,
    string? District,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    Guid? AssignedProviderId,
    bool IsMonitoringActive,
    DateTimeOffset CreatedAt);
