using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Body of the sign-in request.
/// </summary>
/// <param name="Email">Email the account was registered with.</param>
/// <param name="Password">Plain-text password.</param>
public sealed record SignInResource(
    [Required] string Email,
    [Required] string Password);
