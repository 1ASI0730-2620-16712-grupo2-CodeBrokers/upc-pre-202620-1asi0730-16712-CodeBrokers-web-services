using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Notification.Domain.Services;

namespace VitaLink.Platform.API.Notification.Infrastructure.Email;

/// <summary>
///     Email sender adapter for the Resend HTTP API.
/// </summary>
public class ResendEmailSenderService(
    IHttpClientFactory httpClientFactory,
    IOptions<EmailSenderOptions> options,
    ILogger<ResendEmailSenderService> logger) : IEmailSenderService
{
    /// <summary>Name of the configured HTTP client for Resend.</summary>
    public const string HttpClientName = "Resend";

    private readonly EmailSenderOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<DeliveryReceipt> SendAsync(string recipient, MessageContent content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            throw new ArgumentException("Recipient cannot be null, empty or whitespace.", nameof(recipient));
        ArgumentNullException.ThrowIfNull(content);

        var client = httpClientFactory.CreateClient(HttpClientName);

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = JsonContent.Create(new ResendEmailRequest(
            $"{_options.SenderName} <{_options.SenderAddress}>",
            [recipient],
            content.Subject,
            content.Body));

        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Resend rejected the email with status {StatusCode}", (int)response.StatusCode);
            throw new InvalidOperationException($"Email provider returned status {(int)response.StatusCode}.");
        }

        var body = await response.Content.ReadFromJsonAsync<ResendEmailResponse>(cancellationToken);
        if (string.IsNullOrWhiteSpace(body?.Id))
            throw new InvalidOperationException("Email provider did not return a message id.");

        return new DeliveryReceipt(body.Id, DateTimeOffset.UtcNow);
    }

    private sealed record ResendEmailRequest(string From, string[] To, string Subject, string Text);

    private sealed record ResendEmailResponse(string? Id);
}