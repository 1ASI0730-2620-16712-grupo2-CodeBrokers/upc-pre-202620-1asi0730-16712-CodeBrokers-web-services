using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Domain.Model.Commands;

/// <summary>
///     Command to register a new user account.
/// </summary>
/// <param name="Email">Email the user will sign in with.</param>
/// <param name="Password">Plain-text password typed by the user.</param>
/// <param name="Role">Role the account will operate under.</param>
/// <param name="FirstName">First name. Required for care providers.</param>
/// <param name="LastName">Last name. Required for care providers.</param>
/// <param name="Specialty">Medical specialty. Required for care providers.</param>
/// <param name="LicenseNumber">Professional license (4 to 10 digits). Required for care providers.</param>
/// <param name="ClinicId">Clinic the care provider works in. Null means independent.</param>
public sealed record SignUpCommand(
    string Email,
    string Password,
    UserRole Role,
    string? FirstName = null,
    string? LastName = null,
    string? Specialty = null,
    string? LicenseNumber = null,
    Guid? ClinicId = null);
