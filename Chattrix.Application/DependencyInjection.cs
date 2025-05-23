using Microsoft.Extensions.DependencyInjection;
using Chattrix.Application.Interfaces;
using Chattrix.Application.Services;

namespace Chattrix.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IChatService, ChatService>();
        return services;
    }
}

