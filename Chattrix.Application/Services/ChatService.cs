using Chattrix.Application.Interfaces;
using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;

namespace Chattrix.Application.Services;

public class ChatService : IChatService
{
    private readonly IMessageRepository _repository;

    public ChatService(IMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task SendMessageAsync(string user, string content, CancellationToken cancellationToken = default)
    {
        var message = new ChatMessage(Guid.NewGuid(), user, content, DateTime.UtcNow);
        await _repository.AddAsync(message, cancellationToken);
    }

    public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }
}

