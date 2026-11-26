namespace VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Response of a successful sign-in: who signed in and the token to send in later requests.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Email">Email used to sign in.</param>
/// <param name="Role"><c>OLDER_ADULT</c>, <c>FAMILY_CAREGIVER</c> or <c>CARE_PROVIDER</c>.</param>
/// <param name="Token">Token to send as <c>Authorization: Bearer</c>.</param>
/// <param name="ExpiresAt">Moment the token stops being valid.</param>
public sealed record AuthenticatedUserResource(Guid Id, string Email, string Role, string Token, DateTimeOffset ExpiresAt);
