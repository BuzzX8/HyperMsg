using HyperMsg.Buffers;

namespace HyperMsg.Integration.Tests;

public class BufferingIntegrationTests : IntegrationTestsBase
{
    public BufferingIntegrationTests() : base((context, services) => services.AddBufferingContext())
    {
    }

    [Fact]
    public void BufferingContext_ShouldBeAvailable()
    {
        var bufferingContext = GetRequiredService<IBufferingContext>();
        Assert.NotNull(bufferingContext);
    }
}
