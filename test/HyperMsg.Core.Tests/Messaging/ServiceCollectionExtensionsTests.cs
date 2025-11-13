using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg.Messaging;

public class ServiceCollectionExtensionsTests
{
    private readonly ServiceCollection services;

    public ServiceCollectionExtensionsTests()
    {
        services = new ServiceCollection();
    }

    [Fact]
    public void AddMessagingContext_RegistersIMessagingContext()
    {
        // Act
        var builder = services.AddMessagingContext();
        var provider = services.BuildServiceProvider();

        // Assert
        var messagingContext = provider.GetService<IMessagingContext>();
        Assert.NotNull(messagingContext);
        Assert.IsAssignableFrom<IMessagingContext>(messagingContext);
        Assert.IsType<MessagingContextBuilder>(builder);
    }

    [Fact]
    public void AddMessagingContext_RegistersDispatcher()
    {
        // Act
        services.AddMessagingContext();
        var provider = services.BuildServiceProvider();

        // Assert
        var dispatcher = provider.GetService<IDispatcher>();
        Assert.NotNull(dispatcher);
    }

    [Fact]
    public void AddMessagingContext_RegistersHandlerRegistry()
    {
        // Act
        services.AddMessagingContext();
        var provider = services.BuildServiceProvider();

        // Assert
        var registry = provider.GetService<IHandlerRegistry>();
        Assert.NotNull(registry);
    }
}