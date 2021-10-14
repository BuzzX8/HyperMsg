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

        protected ISender Sender => GetRequiredService<ISender>();

        protected IHandlersRegistry HandlersRegistry => GetRequiredService<IHandlersRegistry>();

        protected T GetService<T>() => host.GetService<T>();

        protected T GetRequiredService<T>() => host.GetRequiredService<T>();

        public virtual void Dispose()
        {
            host.Stop();
            host.Dispose();
        }
    }
}
