namespace VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

/// <summary>
///     Confirmation returned by the external provider when it accepts a message.
/// </summary>
public sealed record DeliveryReceipt
{
    /// <summary>
    ///     Creates a delivery receipt.
    /// </summary>
    /// <param name="providerMessageId">Message identifier assigned by the provider.</param>
    /// <param name="acceptedAt">Moment the provider accepted the message.</param>
    /// <exception cref="ArgumentException">Thrown when the provider message id is empty.</exception>
    public DeliveryReceipt(string providerMessageId, DateTimeOffset acceptedAt)
    {
        if (string.IsNullOrWhiteSpace(providerMessageId))
            throw new ArgumentException(
                "Provider message id cannot be null, empty or whitespace.", nameof(providerMessageId));

        ProviderMessageId = providerMessageId;
        AcceptedAt = acceptedAt;
    }

    /// <summary>Message identifier assigned by the provider (for example, the Resend email id).</summary>
    public string ProviderMessageId { get; }

    /// <summary>Moment the provider accepted the message.</summary>
    public DateTimeOffset AcceptedAt { get; }
}