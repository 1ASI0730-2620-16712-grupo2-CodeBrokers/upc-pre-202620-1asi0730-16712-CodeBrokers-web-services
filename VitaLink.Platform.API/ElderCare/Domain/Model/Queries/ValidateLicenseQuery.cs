namespace VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

/// <summary>
///     Query to validate a professional license number.
/// </summary>
/// <param name="LicenseNumber">License number typed by the user.</param>
public sealed record ValidateLicenseQuery(string? LicenseNumber);
