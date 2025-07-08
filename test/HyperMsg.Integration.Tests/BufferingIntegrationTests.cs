using FakeItEasy;
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

    [Fact]
    public async Task BufferingContext_RequestInputBufferHandling_InvokesInputHandlers()
    {
        var inputHandler = A.Fake<BufferHandler>();
        var context = GetRequiredService<IBufferingContext>();
        context.InputHandlers.Add(inputHandler);

        Assert.Contains(inputHandler, context.InputHandlers);
        await context.RequestInputBufferHandling();

        A.CallTo(() => inputHandler(context.Input, A<CancellationToken>._)).MustHaveHappened();
    }
}
