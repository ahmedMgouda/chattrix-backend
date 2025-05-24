using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Chattrix.Api.Hubs;

public class UserHub : Hub
{
    private readonly IUserService _users;

    public UserHub(IUserService users)
    {
        _users = users;
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.GetHttpContext()?.Request.Query["user"].ToString();
        if (!string.IsNullOrEmpty(user))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, user);
            await _users.SetStatusAsync(user, UserStatus.Available);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.GetHttpContext()?.Request.Query["user"].ToString();
        if (!string.IsNullOrEmpty(user))
        {
            await _users.SetStatusAsync(user, UserStatus.Offline);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task UpdateStatus(string user, UserStatus status)
    {
        await _users.SetStatusAsync(user, status);
    }
}

