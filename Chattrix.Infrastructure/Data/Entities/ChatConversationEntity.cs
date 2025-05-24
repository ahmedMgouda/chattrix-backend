using System;
using Chattrix.Core.Models;

namespace Chattrix.Infrastructure.Data;

public class ChatConversationEntity
{
    public Guid Id { get; set; }
    public string User1 { get; set; } = string.Empty;
    public string User2 { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;

    public ChatConversation ToModel() => new(Id, User1, User2, Topic);

    public static ChatConversationEntity FromModel(ChatConversation model) => new()
    {
        Id = model.Id,
        User1 = model.User1,
        User2 = model.User2,
        Topic = model.Topic
    };
}
