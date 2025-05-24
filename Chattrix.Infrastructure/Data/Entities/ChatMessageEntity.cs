using System;
using System.Collections.Generic;
using System.Text.Json;
using Chattrix.Core.Models;

namespace Chattrix.Infrastructure.Data;

public class ChatMessageEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? FilesJson { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }

    public ChatMessage ToModel()
    {
        var files = FilesJson is null ? null :
            JsonSerializer.Deserialize<List<ChatAttachment>>(FilesJson);
        return new ChatMessage(Id, ConversationId, Sender, Recipient, Content, Timestamp, files, IsDelivered, IsRead, IsEdited);
    }

    public static ChatMessageEntity FromModel(ChatMessage model)
    {
        return new ChatMessageEntity
        {
            Id = model.Id,
            ConversationId = model.ConversationId,
            Sender = model.Sender,
            Recipient = model.Recipient,
            Content = model.Content,
            Timestamp = model.Timestamp,
            FilesJson = model.Files is null ? null : JsonSerializer.Serialize(model.Files),
            IsDelivered = model.IsDelivered,
            IsRead = model.IsRead,
            IsEdited = model.IsEdited
        };
    }
}
