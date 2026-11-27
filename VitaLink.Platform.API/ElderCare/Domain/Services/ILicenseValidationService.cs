namespace VitaLink.Platform.API.ElderCare.Domain.Services;

/// <summary>
///     Validates the professional license of a care provider.
/// </summary>
/// <remarks>
///     The implementation is simulated: it checks the format and a registry filled with test data.
///     A real registry can replace it without changing the callers.
/// </remarks>
public interface ILicenseValidationService
{
    Task<LicenseValidationResult> ValidateAsync(
        string? licenseNumber,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Result of validating a license number.
/// </summary>
public enum LicenseValidationResult
{
    /// <summary>The license is registered and free to use.</summary>
    Valid,

    /// <summary>The license does not have 4 to 10 digits.</summary>
    InvalidFormat,

    /// <summary>The license is not in the registry.</summary>
    NotRegistered,

    /// <summary>Another account already uses the license.</summary>
    AlreadyInUse
}
