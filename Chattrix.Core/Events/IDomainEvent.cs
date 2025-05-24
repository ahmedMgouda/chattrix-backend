namespace Chattrix.Core.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
