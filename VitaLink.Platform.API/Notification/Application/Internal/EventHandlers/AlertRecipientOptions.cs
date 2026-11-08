namespace VitaLink.Platform.API.Notification.Application.Internal.EventHandlers;

/// <summary>
///     Configuration of the recipient of alert notifications.
/// </summary>
/// <remarks>
///     For this sprint alerts are sent to a single demo family caregiver, because the
///     real link between an older adult and their family members does not exist yet.
/// </remarks>
public class AlertRecipientOptions
{
    /// <summary>
    ///     Configuration section that holds these options.
    /// </summary>
    public const string SectionName = "Notification:AlertRecipient";

    /// <summary>
    ///     Email of the IAM user that receives alert notifications.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     Whether a recipient email is configured.
    /// </summary>
    public bool HasRecipient => !string.IsNullOrWhiteSpace(Email);
}
