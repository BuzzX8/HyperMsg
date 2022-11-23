using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class KernelTests
{
    public readonly IBuffer buffer;
    public readonly IEncoder encoder;
    public readonly Decoder decoder;
    public readonly Kernel kernel;

    public KernelTests()
    {
        buffer = A.Fake<IBuffer>();
        encoder = A.Fake<IEncoder>();
        decoder = A.Fake<Decoder>();
        kernel = new (decoder, encoder, buffer);
    }

    [Fact]
    public void Dispatch_Invokes_Serializer()
    {
        var message = Guid.NewGuid();

        kernel.Dispatch(message);

        A.CallTo(() => encoder.Encode(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Dispatch_Invokes_SendingBufferUpdated_Event()
    {
        var handler = A.Fake<Action<IBufferReader>>();
        kernel.MessageSerialized += handler;

        kernel.Dispatch(Guid.NewGuid());

        A.CallTo(() => handler.Invoke(buffer.Reader)).MustHaveHappened();
    }

    [Fact]
    public void OnReceivingBufferUpdated_Invokes_Deserializer()
    {
        var receivingBuffer = A.Fake<IBuffer>();

        kernel.ReadBuffer(receivingBuffer.Reader);

        A.CallTo(() => decoder.Invoke(receivingBuffer.Reader, A<IDispatcher>._)).MustHaveHappened();
    }
}
