using HyperMsg.Messaging;

namespace HyperMsg.Hosting;

public class MessagingWorker(IMessagingContext messagingContext, IEnumerable<IMessagingComponent> components, ILogger<MessagingWorker> logger) : BackgroundService
{
    private readonly IMessagingContext messagingContext = messagingContext;
    private readonly IEnumerable<IMessagingComponent> _components = components;
    private readonly ILogger<MessagingWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RegisterHandlersAsync(messagingContext, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await UnregisterHandlersAsync(messagingContext, cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    private Task RegisterHandlersAsync(IMessagingContext messagingContext, CancellationToken cancellationToken)
    {
        // Register components with the messaging context
        foreach (var component in components)
        {
            try
            {
                component.Attach(messagingContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to attach component {ComponentType}", component.GetType().Name);
            }
        }

        return Task.CompletedTask;
    }

    private Task UnregisterHandlersAsync(IMessagingContext messagingContext, CancellationToken cancellationToken)
    {
        // Unregister components from the messaging context
        foreach (var component in _components)
        {
            try
            {
                component.Detach(messagingContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to detach component {ComponentType}", component.GetType().Name);
            }
        }

        return Task.CompletedTask;
    }
}
