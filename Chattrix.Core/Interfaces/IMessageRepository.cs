using Chattrix.Core.Models;

namespace Chattrix.Core.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default);
}

