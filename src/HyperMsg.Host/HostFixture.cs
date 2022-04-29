using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public abstract class HostFixture : IDisposable
{
    private readonly Host host;

    protected HostFixture(Action<IServiceCollection> serviceConfigurator = null)
    {
        host = Host.Create(services =>
        {
            services.AddContext().AddSerializationFilter();
            serviceConfigurator?.Invoke(services);
        });
        host.Start();
    }

    protected IContext Context => GetRequiredService<IContext>();

    protected IForwarder Sender => Context.Sender;

    protected IForwarder Receiver => Context.Receiver;

    protected IRegistry SenderRegistry => Context.Sender.Registry;

    protected IRegistry ReceiverRegistry => Context.Receiver.Registry;

    protected SerializationFilter SerializationFilter => GetRequiredService<SerializationFilter>();

    protected T GetService<T>() => host.GetService<T>();

    protected T GetRequiredService<T>() => host.GetRequiredService<T>();

    public virtual void Dispose()
    {
        host.Stop();
        host.Dispose();
    }
}
