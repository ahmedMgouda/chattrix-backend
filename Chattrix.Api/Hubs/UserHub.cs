using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chattrix.Api.Hubs;

public class UserHub : Hub
{
    private readonly IUserService _users;

    public UserHub(IUserService users)
    {
        _users = users;
    }

    public async Task UpdateStatus(string user, UserStatus status)
    {
        await _users.SetStatusAsync(user, status);
        await Clients.All.SendAsync("StatusChanged", user, status);
    }
}

