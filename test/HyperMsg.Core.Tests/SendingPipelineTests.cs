using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class SendingPipelineTests
{
    public readonly IBuffer buffer;
    public readonly ISerializer serializer;
    public readonly SendingPipeline pipeline;

    public SendingPipelineTests()
    {
        buffer = A.Fake<IBuffer>();
        serializer = A.Fake<ISerializer>();
        pipeline = new SendingPipeline(serializer, buffer);
    }

    [Fact]
    public void Dispatch_Invokes_Serializer()
    {
        var message = Guid.NewGuid();

        pipeline.Dispatch(message);

        A.CallTo(() => serializer.Serialize(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Dispatch_Invokes_Buffer_Handler()
    {
        var handler = A.Fake<Action<IBufferReader>>();
        pipeline.BufferHandler = handler;

        pipeline.Dispatch(Guid.NewGuid());

        A.CallTo(() => handler.Invoke(buffer.Reader)).MustHaveHappened();
    }
}
