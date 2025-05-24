using Microsoft.Extensions.DependencyInjection;
using Chattrix.Core.Interfaces;
using Chattrix.Infrastructure.Repositories;
using Chattrix.Infrastructure.Services;
using Chattrix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Chattrix.Application.Interfaces;

namespace Chattrix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ChatDbContext>(options => options.UseInMemoryDatabase("ChatDb"));
        services.AddScoped<IMessageRepository, DbMessageRepository>();
        services.AddScoped<IConversationRepository, DbConversationRepository>();
        services.AddScoped<IUserRepository, DbUserRepository>();
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        return services;
    }
}

