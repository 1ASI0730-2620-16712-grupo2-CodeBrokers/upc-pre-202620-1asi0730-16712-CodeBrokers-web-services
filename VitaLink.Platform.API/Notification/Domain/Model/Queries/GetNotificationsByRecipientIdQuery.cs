namespace VitaLink.Platform.API.Notification.Domain.Model.Queries;

/// <summary>
///     Query to list the notifications addressed to a recipient, most recent first.
/// </summary>
/// <param name="RecipientId">Recipient user identifier.</param>
public record GetNotificationsByRecipientIdQuery(Guid RecipientId);