using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     Body of the license validation request.
/// </summary>
/// <param name="LicenseNumber">License number to validate.</param>
public sealed record ValidateLicenseResource(
    [Required(ErrorMessage = "ElderCareFieldRequired")] string LicenseNumber);
