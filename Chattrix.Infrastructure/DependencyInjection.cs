using Microsoft.Extensions.DependencyInjection;
using Chattrix.Core.Interfaces;
using Chattrix.Infrastructure.Repositories;

namespace Chattrix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMessageRepository, InMemoryMessageRepository>();
        services.AddSingleton<IConversationRepository, InMemoryConversationRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        return services;
    }
}

