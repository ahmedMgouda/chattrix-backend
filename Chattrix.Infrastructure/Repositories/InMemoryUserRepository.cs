using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using System.Collections.Generic;

namespace Chattrix.Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, UserProfile> _users = new();

    public Task<UserProfile> GetOrCreateAsync(string user, CancellationToken cancellationToken = default)
    {
        if (!_users.TryGetValue(user, out var profile))
        {
            profile = new UserProfile(user);
            _users[user] = profile;
        }

        return Task.FromResult(profile);
    }

    public Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        _users[profile.User] = profile;
        return Task.CompletedTask;
    }
}
