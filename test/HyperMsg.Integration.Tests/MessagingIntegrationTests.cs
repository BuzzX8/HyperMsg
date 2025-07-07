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
    }
}

