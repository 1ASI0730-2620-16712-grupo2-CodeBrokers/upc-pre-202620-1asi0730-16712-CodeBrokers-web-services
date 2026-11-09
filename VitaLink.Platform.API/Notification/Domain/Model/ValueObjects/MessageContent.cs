namespace VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

/// <summary>
///     Subject and body of a notification message.
/// </summary>
public sealed record MessageContent
{
    /// <summary>Maximum number of characters allowed in the subject.</summary>
    public const int MaxSubjectLength = 200;

    /// <summary>
    ///     Creates a message content after validating its fields.
    /// </summary>
    /// <param name="subject">Message subject, up to 200 characters.</param>
    /// <param name="body">Message body.</param>
    /// <exception cref="ArgumentException">Thrown when a field is empty or the subject is too long.</exception>
    public MessageContent(string subject, string body)
    {
        EnsureNotEmpty(subject, nameof(subject));
        EnsureNotEmpty(body, nameof(body));

        if (subject.Length > MaxSubjectLength)
            throw new ArgumentException(
                $"Subject cannot be longer than {MaxSubjectLength} characters.", nameof(subject));

        Subject = subject.Trim();
        Body = body.Trim();
    }

    /// <summary>Message subject.</summary>
    public string Subject { get; }

    /// <summary>Message body.</summary>
    public string Body { get; }

    private static void EnsureNotEmpty(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{parameterName} cannot be null, empty or whitespace.", parameterName);
    }
}