namespace VitaLink.Platform.API.IAM.Domain.Model.Commands;

/// <summary>
///     Command to close the session of the current access token.
/// </summary>
/// <param name="TokenId">The <c>jti</c> of the token to revoke.</param>
/// <param name="ExpiresAt">When the token expires.</param>
public sealed record SignOutCommand(string TokenId, DateTimeOffset ExpiresAt);
