using FakeItEasy;
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

    [Fact]
    public void AddMessagingContext_ReturnsMessagingContextBuilder()
    {
        var builder = services.AddMessagingContext();
        
        Assert.NotNull(builder);
        Assert.Same(services, builder.Services);
    }

    [Fact]
    public void AddMessagingContext_MultipleCalls_RegisterOnlyOnce()
    {
        _ = services.AddMessagingContext();
        _ = services.AddMessagingContext();
        
        var serviceProvider = services.BuildServiceProvider();
        var messagingContexts = serviceProvider.GetServices<IMessagingContext>();
        Assert.Single(messagingContexts);
        var dispatchers = serviceProvider.GetServices<IDispatcher>();
        Assert.Single(dispatchers);
        var handlerRegistries = serviceProvider.GetServices<IHandlerRegistry>();
        Assert.Single(handlerRegistries);
    }

    [Fact]
    public void AddHandler_MessagingContextBuilder_RegistersHandler()
    {
        var builder = services.AddMessagingContext();
        var handler = A.Fake<MessageHandler<string>>();
        var handler2 = A.Fake<MessageHandler<string>>();

        var message = "Test Message";
        builder.AddHandler(handler);
        builder.AddHandler(handler2);

        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        dispatcher.Dispatch(message);

        A.CallTo(() => handler(message)).MustHaveHappenedOnceExactly();
        A.CallTo(() => handler2(message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void AddAsyncHandler_MessagingContextBuilder_RegistersAsyncHandler()
    {
        var builder = services.AddMessagingContext();
        var handler = A.Fake<AsyncMessageHandler<string>>();
        var handler2 = A.Fake<AsyncMessageHandler<string>>();

        var message = "Test Message";
        builder.AddAsyncHandler(handler);
        builder.AddAsyncHandler(handler2);

        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        dispatcher.Dispatch(message);

        A.CallTo(() => handler(message, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => handler2(message, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void AddComponent_MessagingContextBuilder_RegistersComponent()
    {
        var builder = services.AddMessagingContext();
        var component = A.Fake<IMessagingComponent>();
        var component2 = A.Fake<IMessagingComponent>();
        builder.AddComponent(component);
        builder.AddComponent(component2);
        var serviceProvider = services.BuildServiceProvider();
        var components = serviceProvider.GetServices<IMessagingComponent>();
        
        Assert.Contains(component, components);
        Assert.Contains(component2, components);

        var context = serviceProvider.GetRequiredService<IMessagingContext>();

        A.CallTo(() => component.Attach(context)).MustHaveHappenedOnceExactly();
        A.CallTo(() => component2.Attach(context)).MustHaveHappenedOnceExactly();
    }
}