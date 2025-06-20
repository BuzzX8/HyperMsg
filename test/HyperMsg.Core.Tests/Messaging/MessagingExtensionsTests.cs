using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg.Messaging;

public class MessagingExtensionsTests
{
    private readonly ServiceCollection services;

    public MessagingExtensionsTests()
    {
        services = new ServiceCollection();
    }

    [Fact]
    public void AddMessageBroker_ShouldRegisterServices()
    {
        // Act
        services.AdMessagingContext();

        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IDispatcher>());
        Assert.NotNull(serviceProvider.GetService<IHandlerRegistry>());
        Assert.NotNull(serviceProvider.GetService<IMessagingContext>());
    }
}