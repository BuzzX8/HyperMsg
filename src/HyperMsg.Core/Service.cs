using Microsoft.Extensions.Hosting;

namespace HyperMsg;

public abstract class Service : IHostedService
{
    private readonly ITopic topic;

    protected Service(ITopic topic)
    {
        this.topic = topic ?? throw new ArgumentNullException(nameof(topic));
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        RegisterHandlers(topic);
        return Task.CompletedTask;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        UnregisterHandlers(topic);
        return Task.CompletedTask;
    }

    protected abstract void RegisterHandlers(IRegistry registry);

    protected abstract void UnregisterHandlers(IRegistry registry);

    public virtual void Dispose() => UnregisterHandlers(topic);
}
