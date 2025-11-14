using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg.Messaging;

public class ServiceCollectionExtensionsTests
{
    private readonly ServiceCollection services;

    public ServiceCollectionExtensionsTests()
    {
        services = new();
    }

    [Fact]
    public void AddMessagingContext_RegistersServices()
    {
        _ = services.AddMessagingContext();        
        var serviceProvider = services.BuildServiceProvider();

        var messagingContext = serviceProvider.GetService<IMessagingContext>();
        Assert.NotNull(messagingContext);

        var dispatcher = serviceProvider.GetService<IDispatcher>();
        Assert.NotNull(dispatcher);

        var handlerRegistry = serviceProvider.GetService<IHandlerRegistry>();
        Assert.NotNull(handlerRegistry);

        Assert.Same(dispatcher, handlerRegistry);
    }
}