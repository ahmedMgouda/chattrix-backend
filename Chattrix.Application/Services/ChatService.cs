using Chattrix.Application.Interfaces;
using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using System.Linq;

namespace Chattrix.Application.Services;

public class ChatService : IChatService
{
    private readonly IMessageRepository _messages;
    private readonly IConversationRepository _conversations;
    private readonly IUserService _users;

    public ChatService(IMessageRepository messages, IConversationRepository conversations, IUserService users)
    {
        _messages = messages;
        _conversations = conversations;
        _users = users;
    }

    public async Task<Guid> StartConversationAsync(string user1, string user2, string topic, CancellationToken cancellationToken = default)
    {
        var conversation = new ChatConversation(Guid.NewGuid(), user1, user2, topic);
        await _conversations.AddAsync(conversation, cancellationToken);
        return conversation.Id;
    }

    public async Task SendMessageAsync(Guid conversationId, string sender, string content, string? fileName = null, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversations.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null) return;

        var recipient = conversation.User1 == sender ? conversation.User2 : conversation.User1;
        if (await _users.IsBlockedAsync(sender, recipient, cancellationToken)) return;

        var message = new ChatMessage(Guid.NewGuid(), conversationId, sender, recipient, content, DateTime.UtcNow, fileName);
        await _messages.AddAsync(message, cancellationToken);
    }

    public async Task MarkDeliveredAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var msg = await _messages.GetByIdAsync(id, cancellationToken);
        if (msg is null) return;
        await _messages.UpdateAsync(msg with { IsDelivered = true }, cancellationToken);
    }

    public async Task MarkReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var msg = await _messages.GetByIdAsync(id, cancellationToken);
        if (msg is null) return;
        await _messages.UpdateAsync(msg with { IsRead = true }, cancellationToken);
    }

    public async Task UpdateMessageAsync(Guid id, string content, CancellationToken cancellationToken = default)
    {
        var existing = await _messages.GetByIdAsync(id, cancellationToken);
        if (existing is null) return;

        var updated = existing with { Content = content, IsEdited = true };
        await _messages.UpdateAsync(updated, cancellationToken);
    }

    public Task DeleteMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _messages.DeleteAsync(id, cancellationToken);
    }

    public Task<ChatMessage?> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _messages.GetByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return _messages.GetByConversationAsync(conversationId, cancellationToken);
    }

    public async Task<IReadOnlyList<ChatMessage>> SearchAsync(Guid conversationId, string term, CancellationToken cancellationToken = default)
    {
        var messages = await _messages.GetByConversationAsync(conversationId, cancellationToken);
        return messages.Where(m => m.Content.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<IReadOnlyList<string>> GetFilesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await _messages.GetByConversationAsync(conversationId, cancellationToken);
        return messages.Where(m => m.FileName is not null).Select(m => m.FileName!).ToList();
    }
}
