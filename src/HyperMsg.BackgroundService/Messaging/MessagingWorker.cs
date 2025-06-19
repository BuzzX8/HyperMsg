namespace HyperMsg.Messaging;

public class MessagingWorker(MessageBroker messageBroker, ILogger<MessagingWorker> logger) : BackgroundService
{
    private readonly MessageBroker messageBroker = messageBroker;
    private readonly ILogger<MessagingWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RegisterHandlersAsync(messageBroker, messageBroker, stoppingToken);

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
