using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.ElderCare.Domain.Services;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Licensing;

/// <summary>
///     Simulated validation of professional licenses.
/// </summary>
/// <remarks>
///     It does not query a real registry. A license is valid when it has 4 to 10 digits, appears in the
///     <c>license_registry</c> table (test data created by <see cref="Seeding.ElderCareSeeder"/>) and no other
///     account uses it yet. The feature is documented as a simulated verification.
/// </remarks>
public class SimulatedLicenseValidationService(
    IRegisteredLicenseRepository registeredLicenseRepository,
    ICareProviderRepository careProviderRepository) : ILicenseValidationService
{
    public async Task<LicenseValidationResult> ValidateAsync(
        string? licenseNumber,
        CancellationToken cancellationToken = default)
    {
        if (!LicenseNumberRules.IsValidFormat(licenseNumber))
            return LicenseValidationResult.InvalidFormat;

        var normalized = LicenseNumberRules.Normalize(licenseNumber)!;

        if (!await registeredLicenseRepository.ExistsByNumberAsync(normalized, cancellationToken))
            return LicenseValidationResult.NotRegistered;

        if (await careProviderRepository.ExistsByLicenseNumberAsync(normalized, cancellationToken))
            return LicenseValidationResult.AlreadyInUse;

        return LicenseValidationResult.Valid;
    }
}
