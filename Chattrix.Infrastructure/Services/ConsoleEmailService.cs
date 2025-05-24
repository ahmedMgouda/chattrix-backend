using Chattrix.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chattrix.Infrastructure.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending email to {To}: {Subject}\n{Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
