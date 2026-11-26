namespace VitaLink.Platform.API.IAM.Application.Results;

/// <summary>
///     Signed token a client sends to prove who it is.
/// </summary>
/// <param name="Value">Token text.</param>
/// <param name="ExpiresAt">Moment the token stops being valid.</param>
public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);
