using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.IAM.Application.Results;

/// <summary>
///     Outcome of a successful sign-in: the user and the token issued to them.
/// </summary>
/// <param name="User">The signed-in user.</param>
/// <param name="Token">Token issued for this sign-in.</param>
public sealed record AuthenticatedUser(User User, AccessToken Token);
