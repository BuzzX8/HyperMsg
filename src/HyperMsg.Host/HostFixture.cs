using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public abstract class HostFixture : IDisposable
{
    private readonly Host host;

    protected HostFixture(Action<IServiceCollection> serviceConfigurator = null)
    {
        host = Host.Create(services =>
        {
            services.AddCompositeSerializer();
            serviceConfigurator?.Invoke(services);
        });
        host.Start();
    }

    protected CompositeSerializer SerializationFilter => GetRequiredService<CompositeSerializer>();

    protected T GetService<T>() => host.GetService<T>();

    protected T GetRequiredService<T>() => host.GetRequiredService<T>();

    public virtual void Dispose()
    {
        host.Stop();
        host.Dispose();
    }
}
