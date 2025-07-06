using HyperMsg.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperMsg.Integration.Tests;

public class BufferingIntegrationTests : IDisposable
{
    private readonly IHost _host;

    public BufferingIntegrationTests()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddBufferingContext();
            })
            .Build();
        _host.StartAsync();
    }

    [Fact]
    public void BufferingContext_ShouldBeAvailable()
    {
        var bufferingContext = _host.Services.GetService<IBufferingContext>();
        Assert.NotNull(bufferingContext);
    }

    void IDisposable.Dispose()
    {
        _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        _host.Dispose();
    }
}
