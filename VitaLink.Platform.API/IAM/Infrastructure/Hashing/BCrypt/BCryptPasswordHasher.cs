using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Domain.Services;

namespace VitaLink.Platform.API.IAM.Infrastructure.Hashing.BCrypt;

/// <summary>
///     <see cref="IPasswordHasher"/> implementation based on BCrypt.
/// </summary>
/// <remarks>
///     BCrypt adds a random salt to every hash and is deliberately slow, which protects
///     stored passwords against brute-force attacks. The work factor sets that cost.
/// </remarks>
public class BCryptPasswordHasher : IPasswordHasher
{
    /// <summary>BCrypt work factor (2^12 rounds).</summary>
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public PasswordHash Hash(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            throw new ArgumentException("Password cannot be null, empty or whitespace.", nameof(plainText));

        return new PasswordHash(global::BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor));
    }

    /// <inheritdoc />
    public bool Matches(string plainText, PasswordHash hash)
    {
        ArgumentNullException.ThrowIfNull(hash);
        if (string.IsNullOrEmpty(plainText)) return false;

        return global::BCrypt.Net.BCrypt.Verify(plainText, hash.Value);
    }
}
