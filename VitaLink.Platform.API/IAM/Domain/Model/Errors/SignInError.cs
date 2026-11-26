namespace VitaLink.Platform.API.IAM.Domain.Model.Errors;

/// <summary>
///     Reasons why a sign-in can fail.
/// </summary>
public enum SignInError
{
    /// <summary>The email or the password is wrong. Both cases look the same on purpose.</summary>
    InvalidCredentials,

    /// <summary>The credentials are right but the account is deactivated or not verified.</summary>
    AccountDisabled,

    /// <summary>Unexpected failure.</summary>
    UnexpectedError
}
