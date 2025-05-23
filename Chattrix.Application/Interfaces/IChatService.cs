using Chattrix.Core.Models;

namespace Chattrix.Application.Interfaces;

public interface IChatService
{
    Task SendMessageAsync(string user, string content, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = default);
}

