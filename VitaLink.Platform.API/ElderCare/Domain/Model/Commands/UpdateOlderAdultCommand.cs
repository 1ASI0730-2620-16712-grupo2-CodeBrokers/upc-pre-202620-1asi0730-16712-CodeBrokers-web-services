namespace VitaLink.Platform.API.ElderCare.Domain.Model.Commands;

/// <summary>
///     Command to update the profile of an older adult. It replaces every profile field.
/// </summary>
/// <param name="OlderAdultId">Identifier of the older adult.</param>
/// <param name="FirstName">First name.</param>
/// <param name="LastName">Last name.</param>
/// <param name="BirthDate">Birth date (not in the future).</param>
/// <param name="Street">Street of the address.</param>
/// <param name="District">District of the address.</param>
/// <param name="City">City of the address.</param>
/// <param name="EmergencyContactName">Name of the emergency contact.</param>
/// <param name="EmergencyContactPhone">Phone of the emergency contact.</param>
/// <param name="IsMonitoringActive">Whether the monitoring is activated; it needs the complete profile.</param>
public sealed record UpdateOlderAdultCommand(
    Guid OlderAdultId,
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Street,
    string? District,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    bool IsMonitoringActive);
