using Chattrix.Application.Interfaces;
using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;

namespace Chattrix.Api.Hubs;

public class UserHub : Hub
{
    private readonly IUserService _users;
    private readonly IConversationRepository _conversations;

    public UserHub(IUserService users, IConversationRepository conversations)
    {
        _users = users;
        _conversations = conversations;
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.GetHttpContext()?.Request.Query["user"].ToString();
        if (!string.IsNullOrEmpty(user))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, user);
            await _users.SetStatusAsync(user, UserStatus.Available);
            await NotifyContactsAsync(user, UserStatus.Available);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.GetHttpContext()?.Request.Query["user"].ToString();
        if (!string.IsNullOrEmpty(user))
        {
            await _users.SetStatusAsync(user, UserStatus.Offline);
            await NotifyContactsAsync(user, UserStatus.Offline);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task UpdateStatus(string user, UserStatus status)
    {
        await _users.SetStatusAsync(user, status);
        await NotifyContactsAsync(user, status);
    }

    private async Task NotifyContactsAsync(string user, UserStatus status)
    {
        var contacts = await _conversations.GetContactsAsync(user);
        foreach (var contact in contacts.Append(user))
        {
            await Clients.Group(contact).SendAsync("StatusChanged", user, status);
        }
    }
}

