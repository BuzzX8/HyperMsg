using HyperMsg.Messaging;

namespace HyperMsg.Hosting;

public class MessagingWorker(IMessagingContext messagingContext, IEnumerable<IMessagingComponent> components) : BackgroundService
{
    private readonly IMessagingContext messagingContext = messagingContext;
    private readonly IMessagingComponent[] components = [.. components];
    //private readonly ILogger<MessagingWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RegisterHandlersAsync(messagingContext, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await OnHeartBeatAsync(stoppingToken);
        }
    }

    protected virtual Task RegisterHandlersAsync(IMessagingContext messagingContext, CancellationToken cancellationToken)
    {
        // Register components with the messaging context
        for (int i = 0; i < components.Length; i++)
        {
            var component = components[i];
            component.Attach(messagingContext);
        }

        return Task.CompletedTask;
    }

    protected virtual Task UnregisterHandlersAsync(IMessagingContext messagingContext, CancellationToken cancellationToken)
    {
        // Unregister components from the messaging context
        for (int i = 0; i < components.Length; i++)
        {
            var component = components[i];
            component.Detach(messagingContext);
        }
        return Task.CompletedTask;
    }

    protected virtual Task OnHeartBeatAsync(CancellationToken cancellationToken)
    {
        // Override this method in derived classes to handle heartbeat events
        //_logger.LogInformation("Heartbeat event triggered at {Time}", DateTimeOffset.Now);
        return Task.CompletedTask;
    }
}
