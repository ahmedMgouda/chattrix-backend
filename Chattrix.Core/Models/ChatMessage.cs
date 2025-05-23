namespace Chattrix.Core.Models;

/// <summary>
/// Represents a single chat message.
/// </summary>
/// <param name="Id">Unique message identifier.</param>
/// <param name="ConversationId">The conversation this message belongs to.</param>
/// <param name="Sender">Sending user.</param>
/// <param name="Recipient">Receiving user.</param>
/// <param name="Content">Text content.</param>
/// <param name="Timestamp">Creation time.</param>
/// <param name="FileName">Optional file attachment name.</param>
/// <param name="IsDelivered">Whether the message has been delivered.</param>
/// <param name="IsRead">Whether the message has been read by the recipient.</param>
/// <param name="IsEdited">Indicates whether the message has been edited.</param>
public record ChatMessage(
    Guid Id,
    Guid ConversationId,
    string Sender,
    string Recipient,
    string Content,
    DateTime Timestamp,
    string? FileName = null,
    bool IsDelivered = false,
    bool IsRead = false,
    bool IsEdited = false);
