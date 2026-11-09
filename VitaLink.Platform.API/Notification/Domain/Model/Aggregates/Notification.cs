using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Notification.Domain.Model.Aggregates;

/// <summary>
///     Notification aggregate root: a message sent to a recipient through an external channel,
///     with its delivery attempts and result.
/// </summary>
public class Notification
{
    /// <summary>Maximum number of delivery attempts allowed for a notification.</summary>
    public const int MaxAttempts = 5;

    /// <summary>Maximum number of characters stored for a failure reason.</summary>
    public const int MaxFailureReasonLength = 300;

    /// <summary>Required by Entity Framework Core.</summary>
    protected Notification()
    {
        Id = null!;
        Content = null!;
    }

    /// <summary>
    ///     Creates a pending notification for a recipient.
    /// </summary>
    /// <param name="recipientId">Identifier of the recipient user (IAM bounded context).</param>
    /// <param name="channel">Delivery channel.</param>
    /// <param name="content">Message subject and body.</param>
    /// <exception cref="ArgumentException">Thrown when the recipient id is empty.</exception>
    public Notification(Guid recipientId, NotificationChannel channel, MessageContent content)
    {
        if (recipientId == Guid.Empty)
            throw new ArgumentException("Recipient id cannot be empty.", nameof(recipientId));
        ArgumentNullException.ThrowIfNull(content);

        Id = NotificationId.New();
        RecipientId = recipientId;
        Channel = channel;
        Content = content;
        Status = DeliveryStatus.Pending;
        Attempts = 0;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Notification identifier.</summary>
    public NotificationId Id { get; private set; }

    /// <summary>Identifier of the recipient user (reference by id to IAM).</summary>
    public Guid RecipientId { get; private set; }

    /// <summary>Identifier of the alert that originated the notification, if any (reference by id to Alerting).</summary>
    public Guid? RelatedAlertId { get; private set; }

    /// <summary>Delivery channel.</summary>
    public NotificationChannel Channel { get; private set; }

    /// <summary>Message subject and body.</summary>
    public MessageContent Content { get; private set; }

    /// <summary>Current delivery status.</summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>Number of delivery attempts made so far.</summary>
    public int Attempts { get; private set; }

    /// <summary>Message identifier returned by the provider once sent.</summary>
    public string? ProviderMessageId { get; private set; }

    /// <summary>Creation moment.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Moment the provider accepted the message.</summary>
    public DateTimeOffset? SentAt { get; private set; }

    /// <summary>Reason of the last failed attempt.</summary>
    public string? FailureReason { get; private set; }

    /// <summary>
    ///     Records a successful delivery using the provider receipt.
    /// </summary>
    /// <param name="receipt">Receipt returned by the provider.</param>
    public void MarkAsSent(DeliveryReceipt receipt)
    {
        ArgumentNullException.ThrowIfNull(receipt);
        EnsureNotAlreadySent();
        EnsureAttemptsAvailable();

        Attempts++;
        Status = DeliveryStatus.Sent;
        ProviderMessageId = receipt.ProviderMessageId;
        SentAt = receipt.AcceptedAt;
        FailureReason = null;
    }

    /// <summary>
    ///     Records a failed delivery attempt.
    /// </summary>
    /// <param name="reason">Failure description (truncated to 300 characters).</param>
    public void MarkAsFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be null, empty or whitespace.", nameof(reason));
        EnsureNotAlreadySent();
        EnsureAttemptsAvailable();

        Attempts++;
        Status = DeliveryStatus.Failed;
        FailureReason = reason.Length > MaxFailureReasonLength ? reason[..MaxFailureReasonLength] : reason;
    }

    /// <summary>
    ///     Indicates whether a failed notification can be sent again.
    /// </summary>
    /// <param name="maxAttempts">Attempt limit to evaluate (defaults to 5).</param>
    public bool CanRetry(int maxAttempts = MaxAttempts) =>
        Status == DeliveryStatus.Failed && Attempts < Math.Min(maxAttempts, MaxAttempts);

    /// <summary>
    ///     Links the notification to the alert that originated it.
    /// </summary>
    /// <param name="alertId">Alert identifier (Alerting bounded context).</param>
    public void LinkToAlert(Guid alertId)
    {
        if (alertId == Guid.Empty)
            throw new ArgumentException("Alert id cannot be empty.", nameof(alertId));
        if (RelatedAlertId is not null && RelatedAlertId != alertId)
            throw new InvalidOperationException("Notification is already linked to another alert.");

        RelatedAlertId = alertId;
    }

    /// <summary>
    ///     Indicates whether the provider already accepted the message.
    /// </summary>
    public bool IsDelivered() => Status == DeliveryStatus.Sent;

    private void EnsureNotAlreadySent()
    {
        if (IsDelivered())
            throw new InvalidOperationException("Notification has already been sent.");
    }

    private void EnsureAttemptsAvailable()
    {
        if (Attempts >= MaxAttempts)
            throw new InvalidOperationException($"Notification reached the maximum of {MaxAttempts} attempts.");
    }
}