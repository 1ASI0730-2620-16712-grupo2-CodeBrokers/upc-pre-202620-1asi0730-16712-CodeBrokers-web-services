namespace VitaLink.Platform.API.IAM.Domain.Model.Errors;

/// <summary>
///     Reasons why a sign-up can fail.
/// </summary>
public enum SignUpError
{
    /// <summary>The email is empty, too long or not a valid address.</summary>
    InvalidEmail,

    /// <summary>The password does not meet the password policy.</summary>
    InvalidPassword,

    /// <summary>The role cannot be registered through sign-up.</summary>
    RoleNotAllowed,

    /// <summary>Another account already uses the email.</summary>
    EmailAlreadyRegistered,

    /// <summary>Unexpected failure.</summary>
    UnexpectedError
}
