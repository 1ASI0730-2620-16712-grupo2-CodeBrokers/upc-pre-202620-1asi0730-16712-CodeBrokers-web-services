namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     A care provider returned by the API. A null clinic means the provider is independent.
/// </summary>
public sealed record CareProviderResource(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialty,
    Guid? ClinicId,
    string? ClinicName);
