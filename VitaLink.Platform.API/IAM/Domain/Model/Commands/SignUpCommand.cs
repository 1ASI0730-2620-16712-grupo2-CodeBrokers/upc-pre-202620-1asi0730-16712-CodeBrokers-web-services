using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Domain.Model.Commands;

/// <summary>
///     Command to register a new user account.
/// </summary>
/// <param name="Email">Email the user will sign in with.</param>
/// <param name="Password">Plain-text password typed by the user.</param>
/// <param name="Role">Role the account will operate under.</param>
public sealed record SignUpCommand(string Email, string Password, UserRole Role);
