using Chattrix.Core.Models;

namespace Chattrix.Core.Events;

public record ChatMessageSentEvent(ChatMessage Message) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
