using Chattrix.Application.Interfaces;
using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using Chattrix.Core.Events;

namespace Chattrix.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task SetStatusAsync(string user, UserStatus status, CancellationToken cancellationToken = default)
    {
        var profile = await _repository.GetOrCreateAsync(user, cancellationToken);
        profile.Status = status;
        await _repository.UpdateAsync(profile, cancellationToken);
        await DomainEvents.RaiseAsync(new UserStatusChangedEvent(user, status), cancellationToken);
    }

    public async Task<UserStatus> GetStatusAsync(string user, CancellationToken cancellationToken = default)
    {
        var profile = await _repository.GetOrCreateAsync(user, cancellationToken);
        return profile.Status;
    }

    public async Task BlockAsync(string user, string blockedUser, CancellationToken cancellationToken = default)
    {
        var profile = await _repository.GetOrCreateAsync(user, cancellationToken);
        profile.BlockedUserIds.Add(blockedUser);
        await _repository.UpdateAsync(profile, cancellationToken);
    }

    public async Task UnblockAsync(string user, string blockedUser, CancellationToken cancellationToken = default)
    {
        var profile = await _repository.GetOrCreateAsync(user, cancellationToken);
        profile.BlockedUserIds.Remove(blockedUser);
        await _repository.UpdateAsync(profile, cancellationToken);
    }

    public async Task<bool> IsBlockedAsync(string user, string otherUser, CancellationToken cancellationToken = default)
    {
        var profile = await _repository.GetOrCreateAsync(otherUser, cancellationToken);
        return profile.BlockedUserIds.Contains(user);
    }
}
