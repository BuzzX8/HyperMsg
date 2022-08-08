using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class PipelineTests
{
    public readonly IBuffer buffer;
    public readonly ISerializer serializer;
    public readonly Deserializer deserializer;
    public readonly Pipeline pipeline;

    public PipelineTests()
    {
        buffer = A.Fake<IBuffer>();
        serializer = A.Fake<ISerializer>();
        deserializer = A.Fake<Deserializer>();
        pipeline = new (deserializer, serializer, buffer);
    }

    [Fact]
    public void Dispatch_Invokes_Serializer()
    {
        var message = Guid.NewGuid();

        pipeline.Dispatch(message);

        A.CallTo(() => serializer.Serialize(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Dispatch_Invokes_SendingBufferUpdated_Event()
    {
        var handler = A.Fake<Action<IBufferReader>>();
        pipeline.SendingBufferUpdated += handler;

        pipeline.Dispatch(Guid.NewGuid());

        A.CallTo(() => handler.Invoke(buffer.Reader)).MustHaveHappened();
    }

    [Fact]
    public void OnReceivingBufferUpdated_Invokes_Deserializer()
    {
        var receivingBuffer = A.Fake<IBuffer>();

        pipeline.OnReceivingBufferUpdated(receivingBuffer);

        A.CallTo(() => deserializer.Invoke(receivingBuffer.Reader, A<IDispatcher>._)).MustHaveHappened();
    }
}
