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
    public void AddMessageBroker_ShouldRegisterServices()
    {
        // Act
        services.AddMessagingContext();

        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IDispatcher>());
        Assert.NotNull(serviceProvider.GetService<IHandlerRegistry>());
        Assert.NotNull(serviceProvider.GetService<IMessagingContext>());
    }

    [Fact]
    public void AddMessagingComponent_Registers_Component()
    {
        var component = A.Fake<IMessagingComponent>();
        services.AddMessagingComponent(component);

        var serviceProvider = services.BuildServiceProvider();
        var registeredComponent = serviceProvider.GetService<IMessagingComponent>();

        Assert.NotNull(registeredComponent);
        Assert.Same(component, registeredComponent);
    }

    [Fact]
    public void AddMessageHandler_Registers_Configurator()
    {
        // Arrange
        var handler = A.Fake<MessageHandler<string>>();
        services.AddMessageHandler(handler);
        services.AddMessagingContext();

        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetService<IDispatcher>();

        // Act
        dispatcher.Dispatch("test message");

        // Assert
        A.CallTo(handler).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void AddAsyncMessageHandler_Registers_Configurator()
        {
        // Arrange
        var asyncHandler = A.Fake<AsyncMessageHandler<string>>();
        services.AddAsyncMessageHandler(asyncHandler);
        services.AddMessagingContext();

        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetService<IDispatcher>();

        // Act
        dispatcher.DispatchAsync("test message").GetAwaiter().GetResult();

        // Assert
        A.CallTo(() => asyncHandler.Invoke("test message", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}