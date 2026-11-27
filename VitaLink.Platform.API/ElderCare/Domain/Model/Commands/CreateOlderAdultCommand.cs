namespace VitaLink.Platform.API.ElderCare.Domain.Model.Commands;

/// <summary>
///     Command to register an older adult.
/// </summary>
/// <param name="FirstName">First name.</param>
/// <param name="LastName">Last name.</param>
/// <param name="BirthDate">Birth date (not in the future).</param>
/// <param name="Street">Street of the address.</param>
/// <param name="District">District of the address.</param>
/// <param name="City">City of the address.</param>
/// <param name="EmergencyContactName">Name of the emergency contact.</param>
/// <param name="EmergencyContactPhone">Phone of the emergency contact.</param>
/// <param name="IsMonitoringActive">Whether the monitoring is activated; it needs the complete profile.</param>
/// <param name="CareProviderId">Care provider that follows the older adult, when known.</param>
public sealed record CreateOlderAdultCommand(
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Street,
    string? District,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    bool IsMonitoringActive,
    Guid? CareProviderId);
