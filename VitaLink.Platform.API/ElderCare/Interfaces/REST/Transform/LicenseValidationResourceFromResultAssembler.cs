using VitaLink.Platform.API.ElderCare.Domain.Services;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

/// <summary>
///     Converts the result of a license validation into its REST resource.
/// </summary>
public static class LicenseValidationResourceFromResultAssembler
{
    public static LicenseValidationResource ToResourceFromResult(
        string licenseNumber,
        LicenseValidationResult result)
    {
        var reason = result switch
        {
            LicenseValidationResult.InvalidFormat => "INVALID_FORMAT",
            LicenseValidationResult.NotRegistered => "NOT_REGISTERED",
            LicenseValidationResult.AlreadyInUse => "ALREADY_IN_USE",
            _ => null
        };

        return new LicenseValidationResource(
            licenseNumber.Trim(),
            result == LicenseValidationResult.Valid,
            reason);
    }
}
