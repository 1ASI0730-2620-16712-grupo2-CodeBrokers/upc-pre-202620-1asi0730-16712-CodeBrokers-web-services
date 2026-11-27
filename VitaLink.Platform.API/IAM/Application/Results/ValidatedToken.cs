using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Application.Results;

/// <summary>
///     Data read from a valid access token.
/// </summary>
/// <param name="UserId">The signed-in user.</param>
/// <param name="TokenId">Unique identifier of the token (<c>jti</c>), used to revoke it on sign-out.</param>
/// <param name="ExpiresAt">When the token expires.</param>
public sealed record ValidatedToken(UserId UserId, string TokenId, DateTimeOffset ExpiresAt);
