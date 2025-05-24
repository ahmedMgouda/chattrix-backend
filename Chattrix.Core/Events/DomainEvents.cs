using Microsoft.Extensions.DependencyInjection;

namespace Chattrix.Core.Events;

public static class DomainEvents
{
    private static IServiceProvider? _serviceProvider;

    public static void SetServiceProvider(IServiceProvider provider)
    {
        _serviceProvider = provider;
    }

    public static async Task RaiseAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        if (_serviceProvider is null) return;
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IDomainEventHandler<T>>();
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
