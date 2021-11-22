using Microsoft.Extensions.Hosting;

namespace HyperMsg;

/// <summary>
/// Basic implementation fo hosted service
/// </summary>
public abstract class HostedServiceBase : IHostedService, IDisposable
{
    public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual void Dispose()
    { }
}
