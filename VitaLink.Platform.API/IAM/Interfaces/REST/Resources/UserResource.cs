namespace VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Public representation of a user account. It never includes the password hash.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Email">Email used to sign in.</param>
/// <param name="Role"><c>OLDER_ADULT</c>, <c>FAMILY_CAREGIVER</c> or <c>CARE_PROVIDER</c>.</param>
/// <param name="IsVerified">Whether the account is verified.</param>
public sealed record UserResource(Guid Id, string Email, string Role, bool IsVerified);
