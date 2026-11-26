using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Body of the sign-up request.
/// </summary>
/// <param name="Email">Email the user will sign in with.</param>
/// <param name="Password">Plain-text password.</param>
/// <param name="Role"><c>CARE_PROVIDER</c> or <c>FAMILY_CAREGIVER</c>.</param>
/// <param name="FirstName">First name. Required when the role is <c>CARE_PROVIDER</c>.</param>
/// <param name="LastName">Last name. Required when the role is <c>CARE_PROVIDER</c>.</param>
/// <param name="Specialty">Medical specialty. Required when the role is <c>CARE_PROVIDER</c>.</param>
public sealed record SignUpResource(
    [Required] string Email,
    [Required] string Password,
    [Required] string Role,
    string? FirstName = null,
    string? LastName = null,
    string? Specialty = null);
