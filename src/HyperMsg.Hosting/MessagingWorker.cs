using HyperMsg.Messaging;

namespace HyperMsg.Hosting;

public class MessagingWorker(IMessagingContext messagingContext, ILogger<MessagingWorker> logger) : BackgroundService
{
    private readonly IMessagingContext messagingContext = messagingContext;
    private readonly ILogger<MessagingWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RegisterHandlersAsync(messagingContext.Dispatcher, messagingContext.HandlerRegistry, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await OnHeartBeatAsync(stoppingToken);
        }
    }

    protected virtual Task RegisterHandlersAsync(IDispatcher dispatcher, IHandlerRegistry registry, CancellationToken cancellationToken)
    {
        // Override this method in derived classes to register message handlers
        return Task.CompletedTask;
    }

    public virtual Task OnHeartBeatAsync(CancellationToken cancellationToken)
    {
        // Override this method in derived classes to handle heartbeat events
        return Task.CompletedTask;
    }
}
