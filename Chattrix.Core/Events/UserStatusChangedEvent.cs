using Chattrix.Core.Models;

namespace Chattrix.Core.Events;

public record UserStatusChangedEvent(string User, UserStatus Status) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
