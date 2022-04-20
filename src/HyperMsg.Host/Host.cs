﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperMsg;

public class Host : IHost, IServiceProvider
{
    private readonly ServiceProvider serviceProvider;

    public Host(IServiceCollection services) => serviceProvider = services.BuildServiceProvider();

    public IServiceProvider Services => serviceProvider;

    public object GetService(Type serviceType) => Services.GetService(serviceType);

    public void Dispose() => serviceProvider.Dispose();

    public void Start() => StartAsync().GetAwaiter().GetResult();

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var hostedServices = serviceProvider.GetServices<IHostedService>();

        foreach (var service in hostedServices)
        {
            await service.StartAsync(cancellationToken);
        }
    }

    public void Stop() => StopAsync().GetAwaiter().GetResult();

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var hostedServices = serviceProvider.GetServices<IHostedService>();

        foreach (var service in hostedServices)
        {
            await service.StopAsync(cancellationToken);
        }
    }

    public static Host Create(Action<IServiceCollection> serviceConfigurator)
    {
        var services = new ServiceCollection();
        serviceConfigurator.Invoke(services);

        return new Host(services);
    }

    public static Host CreateDefault(Action<IServiceCollection> serviceConfigurator = null)
    {
        return Create(services =>
        {
            serviceConfigurator?.Invoke(services);
        });
    }
}
