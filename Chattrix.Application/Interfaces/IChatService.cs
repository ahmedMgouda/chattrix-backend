using Chattrix.Core.Models;

namespace Chattrix.Application.Interfaces;

public interface IChatService
{
    Task<Guid> StartConversationAsync(string user1, string user2, string topic, CancellationToken cancellationToken = default);
    Task SendMessageAsync(Guid conversationId, string sender, string content, IReadOnlyList<ChatAttachment>? files = null, CancellationToken cancellationToken = default);
    Task MarkDeliveredAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateMessageAsync(Guid id, string content, CancellationToken cancellationToken = default);
    Task DeleteMessageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChatMessage?> GetMessageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> SearchAsync(Guid conversationId, string term, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatAttachment>> GetFilesAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatConversation>> GetConversationsAsync(string user, CancellationToken cancellationToken = default);
}

