using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class KernelTests
{
    public readonly IBuffer buffer;
    public readonly Encoder serializer;
    public readonly Decoder deserializer;
    public readonly Kernel pipeline;

    public KernelTests()
    {
        buffer = A.Fake<IBuffer>();
        serializer = A.Fake<Encoder>();
        deserializer = A.Fake<Decoder>();
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
        pipeline.MessageSerialized += handler;

        pipeline.Dispatch(Guid.NewGuid());

        A.CallTo(() => handler.Invoke(buffer.Reader)).MustHaveHappened();
    }

    [Fact]
    public void OnReceivingBufferUpdated_Invokes_Deserializer()
    {
        var receivingBuffer = A.Fake<IBuffer>();

        pipeline.ReadBuffer(receivingBuffer.Reader);

        A.CallTo(() => deserializer.Invoke(receivingBuffer.Reader, A<IDispatcher>._)).MustHaveHappened();
    }
}
