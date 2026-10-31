namespace VitaLink.Platform.API.Notification.Infrastructure.Email;

/// <summary>
///     Configuration of the external email provider, bound from the
///     <c>Notification:Email</c> configuration section.
/// </summary>
/// <remarks>
///     <see cref="ApiKey"/> is a secret: it must be set with <c>dotnet user-secrets</c>
///     or an environment variable, never in appsettings files or the repository.
/// </remarks>
public class EmailSenderOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Notification:Email";

    /// <summary>Provider API key. When empty, the logging simulator is used instead of the real provider.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Sender email address (for example, <c>onboarding@resend.dev</c>).</summary>
    public string SenderAddress { get; set; } = string.Empty;

    /// <summary>Sender display name.</summary>
    public string SenderName { get; set; } = "VitaLink";

    /// <summary>Indicates whether a real provider can be used.</summary>
    public bool HasApiKey => !string.IsNullOrWhiteSpace(ApiKey);
}