using System.Text.RegularExpressions;

namespace VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

/// <summary>
///     Email address used by a user to sign in.
/// </summary>
/// <remarks>
///     The value is trimmed and stored in lower case, so the same address always matches
///     regardless of how it was typed. The format rule matches the <c>iam.users</c> check constraint.
/// </remarks>
public sealed partial record Email
{
    /// <summary>Maximum number of characters allowed.</summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     Creates an email address.
    /// </summary>
    /// <param name="value">Email address.</param>
    /// <exception cref="ArgumentException">Thrown when the value is empty, too long or not a valid address.</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null, empty or whitespace.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();
        EnsureValidFormat(normalized);
        Value = normalized;
    }

    /// <summary>Normalized email address.</summary>
    public string Value { get; }

    /// <summary>
    ///     Gets the domain part of the address (after the <c>@</c>).
    /// </summary>
    public string Domain() => Value[(Value.IndexOf('@') + 1)..];

    /// <inheritdoc />
    public override string ToString() => Value;

    private static void EnsureValidFormat(string value)
    {
        if (value.Length > MaxLength)
            throw new ArgumentException($"Email cannot exceed {MaxLength} characters.", nameof(value));
        if (!EmailPattern().IsMatch(value))
            throw new ArgumentException("Email format is not valid.", nameof(value));
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
