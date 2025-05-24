using Chattrix.Core.Events;
using Chattrix.Api.Hubs;
using Chattrix.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace Chattrix.Api.Events;

public class BroadcastUserStatusChangedHandler : IDomainEventHandler<UserStatusChangedEvent>
{
    private readonly IHubContext<UserHub> _hub;
    private readonly IConversationRepository _conversations;

    public BroadcastUserStatusChangedHandler(IHubContext<UserHub> hub, IConversationRepository conversations)
    {
        _hub = hub;
        _conversations = conversations;
    }

    public async Task HandleAsync(UserStatusChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var contacts = await _conversations.GetContactsAsync(domainEvent.User, cancellationToken);
        foreach (var contact in contacts.Append(domainEvent.User))
        {
            await _hub.Clients.Group(contact).SendAsync("StatusChanged", domainEvent.User, domainEvent.Status, cancellationToken);
        }
    }
}
