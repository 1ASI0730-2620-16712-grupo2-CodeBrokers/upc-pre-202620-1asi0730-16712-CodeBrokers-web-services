using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Domain.Services;

namespace VitaLink.Platform.API.ElderCare.Application.Internal.QueryServices;

/// <summary>
///     Handles the validation of professional licenses.
/// </summary>
public class LicenseQueryService(ILicenseValidationService licenseValidationService) : ILicenseQueryService
{
    public async Task<LicenseValidationResult> Handle(
        ValidateLicenseQuery query,
        CancellationToken cancellationToken = default)
    {
        return await licenseValidationService.ValidateAsync(query.LicenseNumber, cancellationToken);
    }
}
