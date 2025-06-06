using Chattrix.Application;
using Chattrix.Infrastructure;
using Chattrix.Core.Events;
using Chattrix.Api.Hubs;
using Serilog;
using Chattrix.Api.Events;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
});
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IDomainEventHandler<ChatMessageSentEvent>, BroadcastChatMessageHandler>();
builder.Services.AddScoped<IDomainEventHandler<UserStatusChangedEvent>, BroadcastUserStatusChangedHandler>();

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors("AllowDevClient");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<UserHub>("/hubs/user");
app.UseHangfireDashboard();

DomainEvents.SetServiceProvider(app.Services);

app.Run();

