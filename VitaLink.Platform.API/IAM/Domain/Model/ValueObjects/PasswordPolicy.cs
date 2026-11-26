using System.Text;

namespace VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

/// <summary>
///     Rules a plain-text password must meet before it is hashed.
/// </summary>
/// <remarks>
///     Length is the only rule (OWASP / NIST): no composition rules. The upper limit is the 72 bytes
///     BCrypt can hash; counting bytes keeps accented characters from silently exceeding it.
/// </remarks>
public static class PasswordPolicy
{
    /// <summary>Minimum number of characters.</summary>
    public const int MinLength = 8;

    /// <summary>Maximum size in bytes (UTF-8), the BCrypt limit.</summary>
    public const int MaxBytes = 72;

    /// <summary>
    ///     Checks whether a password meets the policy.
    /// </summary>
    /// <param name="password">Plain-text password.</param>
    public static bool IsValid(string? password) =>
        !string.IsNullOrEmpty(password)
        && password.Length >= MinLength
        && Encoding.UTF8.GetByteCount(password) <= MaxBytes;
}
