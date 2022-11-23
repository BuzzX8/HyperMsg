using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class CompositeEncoderTests
{
    private readonly CompositeEncoder encoder;
    private readonly IBuffer buffer;

    public CompositeEncoderTests()
    {
        buffer = A.Fake<IBuffer>();
        encoder = new();
    }

    [Fact]
    public void Serialize_Invokes_Registered_Serializer()
    {
        var message = Guid.NewGuid();
        var encoder = A.Fake<Encoder<Guid>>();
        this.encoder.Add(encoder);

        this.encoder.Encode(buffer.Writer, message);

        A.CallTo(() => encoder.Invoke(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Serialize_Does_Not_Invokes_Deregistered_Serializer()
    {
        var message = Guid.NewGuid();
        var encoder = A.Fake<Encoder<Guid>>();
        this.encoder.Add(encoder);
        this.encoder.Remove<Guid>();

        this.encoder.Encode(buffer.Writer, message);

        A.CallTo(() => encoder.Invoke(buffer.Writer, message)).MustNotHaveHappened();
    }
}
