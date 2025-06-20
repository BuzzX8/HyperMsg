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
    public void AddMessagingComponent_()
    {
        var component = A.Fake<IMessagingComponent>();
        services.AddMessagingComponent(component);

        var serviceProvider = services.BuildServiceProvider();
        var registeredComponent = serviceProvider.GetService<IMessagingComponent>();

        Assert.NotNull(registeredComponent);
        Assert.Same(component, registeredComponent);
    }
}