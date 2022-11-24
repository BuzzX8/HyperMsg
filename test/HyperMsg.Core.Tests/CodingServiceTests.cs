using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class CodingServiceTests
{
    public readonly IBuffer buffer;
    public readonly IEncoder encoder;
    public readonly Decoder decoder;
    public readonly CodingService service;

    public CodingServiceTests()
    {
        buffer = A.Fake<IBuffer>();
        encoder = A.Fake<IEncoder>();
        decoder = A.Fake<Decoder>();
        service = new (decoder, encoder, buffer);
    }

    [Fact]
    public void Dispatch_Invokes_Serializer()
    {
        var message = Guid.NewGuid();

        service.Dispatch(message);

        A.CallTo(() => encoder.Encode(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Dispatch_Invokes_SendingBufferUpdated_Event()
    {
        var handler = A.Fake<Action<IBufferReader>>();
        service.MessageEncoded += handler;

        service.Dispatch(Guid.NewGuid());

        A.CallTo(() => handler.Invoke(buffer.Reader)).MustHaveHappened();
    }

    [Fact]
    public void OnReceivingBufferUpdated_Invokes_Deserializer()
    {
        var receivingBuffer = A.Fake<IBuffer>();

        service.TryDecodeMessage(receivingBuffer.Reader);

        A.CallTo(() => decoder.Invoke(receivingBuffer.Reader, A<IDispatcher>._)).MustHaveHappened();
    }
}
