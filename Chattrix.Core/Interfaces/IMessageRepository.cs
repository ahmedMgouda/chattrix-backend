using Chattrix.Core.Models;

namespace Chattrix.Core.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task UpdateAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChatMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
}

