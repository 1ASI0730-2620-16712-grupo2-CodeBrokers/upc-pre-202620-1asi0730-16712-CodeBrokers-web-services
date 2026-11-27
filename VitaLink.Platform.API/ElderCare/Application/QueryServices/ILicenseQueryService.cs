using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Domain.Services;

namespace VitaLink.Platform.API.ElderCare.Application.QueryServices;

/// <summary>
///     Defines the license validation used by the registration form.
/// </summary>
public interface ILicenseQueryService
{
    /// <summary>
    ///     Validates a professional license number (simulated verification).
    /// </summary>
    Task<LicenseValidationResult> Handle(
        ValidateLicenseQuery query,
        CancellationToken cancellationToken = default);
}
