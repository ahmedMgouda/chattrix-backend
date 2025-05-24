using Chattrix.Core.Events;
using Chattrix.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chattrix.Api.Events;

public class BroadcastChatMessageHandler : IDomainEventHandler<ChatMessageSentEvent>
{
    private readonly IHubContext<ChatHub> _hub;

    public BroadcastChatMessageHandler(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    public Task HandleAsync(ChatMessageSentEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var msg = domainEvent.Message;
        return _hub.Clients.Group(msg.ConversationId.ToString())
            .SendAsync("ReceiveMessage", msg.ConversationId, msg.Sender, msg.Content, msg.Files, cancellationToken);
    }
}
