using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Notification.Domain.Services;

namespace VitaLink.Platform.API.Notification.Infrastructure.Email;

/// <summary>
///     Simulated email sender that only writes the message to the application log.
/// </summary>
/// <remarks>
///     Used when no provider API key is configured, so development and tests
///     never call the real provider or require credentials.
/// </remarks>
public class LoggingEmailSenderService(ILogger<LoggingEmailSenderService> logger) : IEmailSenderService
{
    /// <inheritdoc />
    public Task<DeliveryReceipt> SendAsync(string recipient, MessageContent content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            throw new ArgumentException("Recipient cannot be null, empty or whitespace.", nameof(recipient));
        ArgumentNullException.ThrowIfNull(content);
        cancellationToken.ThrowIfCancellationRequested();

        var receipt = new DeliveryReceipt($"simulated-{Guid.NewGuid()}", DateTimeOffset.UtcNow);

        logger.LogInformation(
            "Simulated email {ProviderMessageId} to {Recipient}: \"{Subject}\" ({BodyLength} characters)",
            receipt.ProviderMessageId, MaskEmail(recipient), content.Subject, content.Body.Length);

        return Task.FromResult(receipt);
    }

    private static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        return at <= 1 ? "***" : $"{email[0]}***{email[at..]}";
    }
}