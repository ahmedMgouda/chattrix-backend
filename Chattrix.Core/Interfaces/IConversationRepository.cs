using Chattrix.Core.Models;

namespace Chattrix.Core.Interfaces;

public interface IConversationRepository
{
    Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken = default);
    Task<ChatConversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatConversation>> GetByUsersAsync(string user1, string user2, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetContactsAsync(string user, CancellationToken cancellationToken = default);
}
