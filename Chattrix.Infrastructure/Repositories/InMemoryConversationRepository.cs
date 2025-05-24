using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using System.Linq;

namespace Chattrix.Infrastructure.Repositories;

public class InMemoryConversationRepository : IConversationRepository
{
    private readonly List<ChatConversation> _conversations = new();

    public Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken = default)
    {
        _conversations.Add(conversation);
        return Task.CompletedTask;
    }

    public Task<ChatConversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conversation = _conversations.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(conversation);
    }

    public Task<IReadOnlyList<ChatConversation>> GetByUsersAsync(string user1, string user2, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ChatConversation> result = _conversations.Where(c =>
            (c.User1 == user1 && c.User2 == user2) || (c.User1 == user2 && c.User2 == user1)).ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<string>> GetContactsAsync(string user, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> result = _conversations
            .Where(c => c.User1 == user || c.User2 == user)
            .Select(c => c.User1 == user ? c.User2 : c.User1)
            .Distinct()
            .ToList();
        return Task.FromResult(result);
    }
}
