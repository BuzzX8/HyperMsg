﻿using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public abstract class HostFixture : IDisposable
{
    private readonly Host host;

    protected HostFixture(Action<IServiceCollection> serviceConfigurator = null)
    {
        host = Host.CreateDefault(serviceConfigurator);
        host.Start();
    }

    protected IForwarder Sender => GetRequiredService<IForwarder>();

    protected IRegistry HandlersRegistry => GetRequiredService<IRegistry>();

    protected T GetService<T>() => host.GetService<T>();

    protected T GetRequiredService<T>() => host.GetRequiredService<T>();

    public virtual void Dispose()
    {
        host.Stop();
        host.Dispose();
    }
}
