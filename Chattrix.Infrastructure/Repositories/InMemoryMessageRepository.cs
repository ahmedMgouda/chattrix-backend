using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using System.Linq;

namespace Chattrix.Infrastructure.Repositories;

public class InMemoryMessageRepository : IMessageRepository
{
    private readonly List<ChatMessage> _messages = new();

    public Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        var index = _messages.FindIndex(m => m.Id == message.Id);
        if (index >= 0)
        {
            _messages[index] = message;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _messages.RemoveAll(m => m.Id == id);
        return Task.CompletedTask;
    }

    public Task<ChatMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(message);
    }

    public Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ChatMessage> result = _messages.ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<ChatMessage>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ChatMessage> result = _messages.Where(m => m.ConversationId == conversationId).ToList();
        return Task.FromResult(result);
    }
}

