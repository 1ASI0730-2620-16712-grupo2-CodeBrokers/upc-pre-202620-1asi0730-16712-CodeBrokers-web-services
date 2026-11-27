namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     Request body to register or update an older adult.
/// </summary>
/// <remarks>
///     The fields are validated by the domain rules (not by annotations) so that every problem is reported
///     together, including the fields required to activate the monitoring.
/// </remarks>
/// <param name="FirstName">First name.</param>
/// <param name="LastName">Last name.</param>
/// <param name="BirthDate">Birth date (yyyy-MM-dd), not in the future.</param>
/// <param name="Street">Street of the address.</param>
/// <param name="District">District of the address.</param>
/// <param name="City">City of the address.</param>
/// <param name="EmergencyContactName">Name of the emergency contact.</param>
/// <param name="EmergencyContactPhone">Phone of the emergency contact.</param>
/// <param name="IsMonitoringActive">Whether the monitoring is activated; it needs the complete profile.</param>
/// <param name="CareProviderId">Care provider that follows the older adult (only when registering).</param>
public sealed record SaveOlderAdultResource(
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
