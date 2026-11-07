namespace VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

/// <summary>
///     Hashed password of a user account.
/// </summary>
/// <remarks>
///     Only the hash is stored; the plain-text password never reaches the domain model.
///     The hash is produced and verified through the <c>IPasswordHasher</c> port.
/// </remarks>
public sealed record PasswordHash
{
    /// <summary>Maximum number of characters allowed.</summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     Creates a password hash.
    /// </summary>
    /// <param name="value">Hash produced by the password hasher.</param>
    /// <exception cref="ArgumentException">Thrown when the value is empty or too long.</exception>
    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password hash cannot be null, empty or whitespace.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Password hash cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    /// <summary>Hash value.</summary>
    public string Value { get; }

    /// <summary>
    ///     Hides the hash in logs and debugging output.
    /// </summary>
    public override string ToString() => "********";
}
