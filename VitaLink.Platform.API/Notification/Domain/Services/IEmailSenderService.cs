using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Notification.Domain.Services;

/// <summary>
///     Port for sending emails through an external provider.
/// </summary>
/// <remarks>
///     The domain depends only on this contract; each provider (Resend, a logging simulator, etc.)
///     is an adapter in the Infrastructure layer.
/// </remarks>
public interface IEmailSenderService
{
    /// <summary>
    ///     Sends an email to a recipient.
    /// </summary>
    /// <param name="recipient">Recipient email address.</param>
    /// <param name="content">Message subject and body.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The receipt issued by the provider when it accepts the message.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the provider rejects or cannot process the message.</exception>
    Task<DeliveryReceipt> SendAsync(string recipient, MessageContent content, CancellationToken cancellationToken = default);
}