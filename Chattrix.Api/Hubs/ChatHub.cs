using System;
using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chattrix.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public Task JoinConversation(Guid conversationId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task SendMessage(Guid conversationId, string sender, string content, IReadOnlyList<ChatAttachment>? files)
    {
        await _chatService.SendMessageAsync(conversationId, sender, content, files);
        await Clients.Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", conversationId, sender, content, files);
    }
}
