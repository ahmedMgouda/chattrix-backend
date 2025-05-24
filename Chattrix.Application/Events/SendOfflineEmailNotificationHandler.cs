using Chattrix.Core.Events;
using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Hangfire;

namespace Chattrix.Application.Events;

public class SendOfflineEmailNotificationHandler : IDomainEventHandler<ChatMessageSentEvent>
{
    private readonly IUserService _users;
    private readonly IEmailService _emails;
    private readonly IBackgroundJobClient _jobs;

    public SendOfflineEmailNotificationHandler(IUserService users, IEmailService emails, IBackgroundJobClient jobs)
    {
        _users = users;
        _emails = emails;
        _jobs = jobs;
    }

    public async Task HandleAsync(ChatMessageSentEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var recipient = domainEvent.Message.Recipient;
        var status = await _users.GetStatusAsync(recipient, cancellationToken);
        if (status == UserStatus.Offline)
        {
            _jobs.Enqueue<IEmailService>(e => e.SendEmailAsync(recipient, $"New message from {domainEvent.Message.Sender}", domainEvent.Message.Content, CancellationToken.None));
        }
    }
}
