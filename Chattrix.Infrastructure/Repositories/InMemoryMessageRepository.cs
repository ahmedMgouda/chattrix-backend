using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;

namespace Chattrix.Infrastructure.Repositories;

public class InMemoryMessageRepository : IMessageRepository
{
    private readonly List<ChatMessage> _messages = new();

    public Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ChatMessage> result = _messages.ToList();
        return Task.FromResult(result);
    }
}

