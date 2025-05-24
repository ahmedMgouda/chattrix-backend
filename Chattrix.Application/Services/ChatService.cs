using Chattrix.Application.Interfaces;
using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using Chattrix.Core.Events;
using Hangfire;
using System.Linq;
using System.IO;

namespace Chattrix.Application.Services;

public class ChatService : IChatService
{
    private readonly IMessageRepository _messages;
    private readonly IConversationRepository _conversations;
    private readonly IUserService _users;
    private readonly IEmailService _emails;
    private readonly IBackgroundJobClient _jobs;
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB default limit
    // Allowed file types for attachments, including audio for voice messages
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".txt",
        ".mp3", ".wav", ".ogg", ".m4a"
    };

    public ChatService(
        IMessageRepository messages,
        IConversationRepository conversations,
        IUserService users,
        IEmailService emails,
        IBackgroundJobClient jobs)
    {
        _messages = messages;
        _conversations = conversations;
        _users = users;
        _emails = emails;
        _jobs = jobs;
    }

    public async Task<Guid> StartConversationAsync(string user1, string user2, string topic, CancellationToken cancellationToken = default)
    {
        var conversation = new ChatConversation(Guid.NewGuid(), user1, user2, topic);
        await _conversations.AddAsync(conversation, cancellationToken);
        return conversation.Id;
    }

    public async Task SendMessageAsync(Guid conversationId, string sender, string content, IReadOnlyList<ChatAttachment>? files = null, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversations.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null) return;

        var recipient = conversation.User1 == sender ? conversation.User2 : conversation.User1;
        if (await _users.IsBlockedAsync(sender, recipient, cancellationToken)) return;

        if (files is not null)
        {
            foreach (var file in files)
            {
                var ext = file.Extension;
                if (!AllowedExtensions.Contains(ext))
                {
                    throw new InvalidOperationException($"Files of type '{ext}' are not allowed.");
                }
                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(file.Data);
                }
                catch (FormatException)
                {
                    throw new InvalidOperationException($"Attachment '{file.FileName}' is not valid base64.");
                }
                if (bytes.Length > MaxFileSizeBytes)
                {
                    throw new InvalidOperationException($"File '{file.FileName}' exceeds allowed size of {MaxFileSizeBytes} bytes.");
                }
            }
        }

        var message = new ChatMessage(Guid.NewGuid(), conversationId, sender, recipient, content, DateTime.UtcNow, files);
        await _messages.AddAsync(message, cancellationToken);

        await DomainEvents.RaiseAsync(new ChatMessageSentEvent(message), cancellationToken);
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

    public async Task<IReadOnlyList<ChatAttachment>> GetFilesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await _messages.GetByConversationAsync(conversationId, cancellationToken);
        return messages.SelectMany(m => m.Files ?? Array.Empty<ChatAttachment>()).ToList();
    }
}
