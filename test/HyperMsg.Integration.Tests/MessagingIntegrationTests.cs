using FakeItEasy;
using HyperMsg.Messaging;

namespace HyperMsg.Integration.Tests;

public class MessagingIntegrationTests : IntegrationTestsBase
{
    public MessagingIntegrationTests() : base((_, services) => services.AddMessagingContext())
    { }

    [Fact]
    public void MessagingContext_ShouldBeAvailable()
    {
        var messagingContext = GetRequiredService<IMessagingContext>();
        
        Assert.NotNull(messagingContext);
        Assert.NotNull(messagingContext.Dispatcher);
        Assert.NotNull(messagingContext.HandlerRegistry);
    }

    [Fact]
    public void MessagingContext_Dispatch_Invokes_Registered_Handlers()
    {
        var messagingContext = GetRequiredService<IMessagingContext>();
        var handler = A.Fake<MessageHandler<Guid>>();
        messagingContext.HandlerRegistry.Register(handler);
        var message = Guid.NewGuid();

        messagingContext.Dispatcher.Dispatch(message);

        A.CallTo(() => handler.Invoke(message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task MessagingContext_DispatchAsync_Invokes_Registered_Handlers()
    {
        var messagingContext = GetRequiredService<IMessagingContext>();
        var handler = A.Fake<AsyncMessageHandler<Guid>>();
        messagingContext.HandlerRegistry.Register(handler);
        var message = Guid.NewGuid();

        await messagingContext.Dispatcher.DispatchAsync(message);
        
        A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void MessagingContext_Dispatch_DoesNotInvoke_Unregistered_Handler()
    {
        var messagingContext = GetRequiredService<IMessagingContext>();
        var handler = A.Fake<MessageHandler<Guid>>();
        messagingContext.HandlerRegistry.Register(handler);
        messagingContext.HandlerRegistry.Unregister(handler);
        var message = Guid.NewGuid();

        messagingContext.Dispatcher.Dispatch(message);

        A.CallTo(() => handler.Invoke(message)).MustNotHaveHappened();
    }

    [Fact]
    public void MessagingContext_Unregistering_NonRegistered_Handler_DoesNotThrow()
    {
        var messagingContext = GetRequiredService<IMessagingContext>();
        var handler = A.Fake<MessageHandler<Guid>>();

        // Should not throw even if handler was never registered
        messagingContext.HandlerRegistry.Unregister(handler);
    }
}

