using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class CompositeEncoderTests
{
    private readonly CompositeEncoder serializer;
    private readonly IBuffer buffer;

    public CompositeEncoderTests()
    {
        buffer = A.Fake<IBuffer>();
        serializer = new();
    }

    [Fact]
    public void Serialize_Invokes_Registered_Serializer()
    {
        var message = Guid.NewGuid();
        var serializer = A.Fake<Action<IBufferWriter, Guid>>();
        this.serializer.Register(serializer);

        this.serializer.Encode(buffer.Writer, message);

        A.CallTo(() => serializer.Invoke(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Serialize_Does_Not_Invokes_Deregistered_Serializer()
    {
        var message = Guid.NewGuid();
        var serializer = A.Fake<Action<IBufferWriter, Guid>>();
        this.serializer.Register(serializer);
        this.serializer.Deregister<Guid>();

        this.serializer.Encode(buffer.Writer, message);

        A.CallTo(() => serializer.Invoke(buffer.Writer, message)).MustNotHaveHappened();
    }
}
