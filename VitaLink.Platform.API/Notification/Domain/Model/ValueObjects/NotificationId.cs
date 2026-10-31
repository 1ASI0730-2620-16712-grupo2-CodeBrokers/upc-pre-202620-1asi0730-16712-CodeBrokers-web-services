namespace VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

/// <summary>
///     Unique identifier of a notification.
/// </summary>
public sealed record NotificationId
{
    /// <summary>
    ///     Creates an identifier from an existing value.
    /// </summary>
    /// <param name="value">Non-empty GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the value is empty.</exception>
    public NotificationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("NotificationId cannot be empty.", nameof(value));

        Value = value;
    }

    /// <summary>Underlying GUID value.</summary>
    public Guid Value { get; }

    /// <summary>
    ///     Generates a new random identifier.
    /// </summary>
    public static NotificationId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}