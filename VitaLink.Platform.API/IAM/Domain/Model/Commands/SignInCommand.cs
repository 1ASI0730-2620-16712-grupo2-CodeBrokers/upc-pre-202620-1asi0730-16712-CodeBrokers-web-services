namespace VitaLink.Platform.API.IAM.Domain.Model.Commands;

/// <summary>
///     Command to sign in with an existing account.
/// </summary>
/// <param name="Email">Email the account was registered with.</param>
/// <param name="Password">Plain-text password typed by the user.</param>
public sealed record SignInCommand(string Email, string Password);
