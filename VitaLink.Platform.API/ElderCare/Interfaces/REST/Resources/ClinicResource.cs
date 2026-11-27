namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     A clinic returned by the API.
/// </summary>
public sealed record ClinicResource(
    Guid Id,
    string Name,
    string District);
