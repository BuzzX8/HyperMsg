using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperMsg.Integration.Tests;

public abstract class IntegrationTestsBase : IDisposable
{
    private readonly IHost _host;

    protected IntegrationTestsBase(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(configureDelegate)
            .Build();
        _host.StartAsync().GetAwaiter().GetResult();
    }

    protected IServiceProvider ServiceProvider => _host.Services;

    protected TService GetRequiredService<TService>() where TService : notnull => ServiceProvider.GetRequiredService<TService>();

    public void Dispose()
    {
        _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        _host.Dispose();
    }
}
