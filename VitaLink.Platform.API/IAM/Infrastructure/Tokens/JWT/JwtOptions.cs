namespace VitaLink.Platform.API.IAM.Infrastructure.Tokens.JWT;

/// <summary>
///     Settings used to sign access tokens. Bound from the <c>IAM:Jwt</c> configuration section.
/// </summary>
/// <remarks>
///     The signing key is a secret: it comes from user secrets or an environment variable
///     (<c>IAM__Jwt__SigningKey</c>) and is never committed outside the Development settings.
/// </remarks>
public class JwtOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "IAM:Jwt";

    /// <summary>Minimum length of the signing key (HMAC-SHA256 needs at least 256 bits).</summary>
    public const int MinKeyLength = 32;

    /// <summary>Who issues the tokens.</summary>
    public string Issuer { get; set; } = "VitaLink.Platform.API";

    /// <summary>Who the tokens are meant for.</summary>
    public string Audience { get; set; } = "VitaLink.Platform.Web";

    /// <summary>Secret used to sign the tokens.</summary>
    public string? SigningKey { get; set; }

    /// <summary>Minutes a token stays valid. Tokens cannot be revoked, so they are short-lived.</summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>Whether the settings are complete and safe to use.</summary>
    public bool IsValid => !string.IsNullOrWhiteSpace(SigningKey)
                           && SigningKey.Length >= MinKeyLength
                           && ExpirationMinutes > 0;
}
