using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class CodingServiceTests
{
    public readonly IEncoder encoder;
    public readonly Decoder decoder;
    public readonly CodingService service;

    public CodingServiceTests()
    {
        encoder = A.Fake<IEncoder>();
        decoder = A.Fake<Decoder>();
        service = new (decoder, encoder, 100, 100);
    }

    [Fact]
    public void Dispatching_Message_Invokes_Encoder()
    {
        var message = Guid.NewGuid();

        service.Dispatch(message);

        A.CallTo(() => encoder.Encode(service.EncodingBuffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Dispatching_Message_Invokes_MessageEncoded_Event()
    {
        var handler = A.Fake<Action>();
        service.MessageEncoded += handler;

        service.Dispatch(Guid.NewGuid());

        A.CallTo(() => handler.Invoke()).MustHaveHappened();
    }

    [Fact]
    public void DecodeMessage_Invokes_Decoder()
    {
        var receivingBuffer = A.Fake<IBuffer>();

        service.DecodeMessage();

        A.CallTo(() => decoder.Invoke(service.DecodingBuffer.Reader, A<IDispatcher>._)).MustHaveHappened();
    }
}
