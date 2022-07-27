using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public abstract class HostFixture : IDisposable
{
    private readonly Host host;

    protected HostFixture(Action<IServiceCollection> serviceConfigurator = null)
    {
        host = Host.Create(services =>
        {
            services.AddSerializationFilter();
            serviceConfigurator?.Invoke(services);
        });
        host.Start();
    }

    protected SerializationFilter SerializationFilter => GetRequiredService<SerializationFilter>();

    protected T GetService<T>() => host.GetService<T>();

    protected T GetRequiredService<T>() => host.GetRequiredService<T>();

    public virtual void Dispose()
    {
        host.Stop();
        host.Dispose();
    }
}
