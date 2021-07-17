using Microsoft.Extensions.DependencyInjection;
using System;

namespace HyperMsg
{
    public abstract class HostFixture : IDisposable
    {
        private readonly Host host;

        protected HostFixture(Action<IServiceCollection> serviceConfigurator = null)
        {
            host = Host.CreateDefault(serviceConfigurator);
            host.Start();
        }

        protected IMessagingContext MessagingContext => GetRequiredService<IMessagingContext>();

        protected IMessageSender MessageSender => GetRequiredService<IMessageSender>();

        protected IMessageHandlersRegistry HandlersRegistry => GetRequiredService<IMessageHandlersRegistry>();

        protected T GetService<T>() => host.GetService<T>();

        protected T GetRequiredService<T>() => host.GetRequiredService<T>();

        public virtual void Dispose()
        {
            host.Stop();
            host.Dispose();
        }
    }
}
