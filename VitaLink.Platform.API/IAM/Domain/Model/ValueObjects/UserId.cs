namespace VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

/// <summary>
///     Unique identifier of a user account.
/// </summary>
/// <remarks>
///     Other bounded contexts reference users only by this identifier's value (<see cref="Guid"/>).
/// </remarks>
public sealed record UserId
{
    /// <summary>
    ///     Creates an identifier from an existing value.
    /// </summary>
    /// <param name="value">Non-empty GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the value is empty.</exception>
    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(value));

        Value = value;
    }

    /// <summary>Underlying GUID value.</summary>
    public Guid Value { get; }

    /// <summary>
    ///     Generates a new random identifier.
    /// </summary>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    ///     Creates an identifier from its text representation.
    /// </summary>
    /// <param name="value">GUID in text form.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not a valid, non-empty GUID.</exception>
    public static UserId Of(string value) =>
        Guid.TryParse(value, out var guid)
            ? new UserId(guid)
            : throw new ArgumentException("UserId must be a valid GUID.", nameof(value));

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
