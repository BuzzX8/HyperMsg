using HyperMsg.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperMsg.Integration.Tests;

public class MessagingIntegrationTests : IDisposable
{
    private readonly IHost _host;

    public MessagingIntegrationTests() 
    { 
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddMessagingContext();
            })
            .Build();
        _host.StartAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public void MessagingContext_ShouldBeAvailable()
    {
        var messagingContext = _host.Services.GetService<IMessagingContext>();
        Assert.NotNull(messagingContext);
    }

    public void Dispose()
    {
        _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        _host.Dispose();
    }
}

