namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     Result of validating a license number.
/// </summary>
/// <param name="LicenseNumber">The license number that was checked.</param>
/// <param name="IsValid">Whether the license can be used to register.</param>
/// <param name="Reason">
///     <c>INVALID_FORMAT</c>, <c>NOT_REGISTERED</c> or <c>ALREADY_IN_USE</c> when the license is not valid.
/// </param>
public sealed record LicenseValidationResource(
    string LicenseNumber,
    bool IsValid,
    string? Reason);
