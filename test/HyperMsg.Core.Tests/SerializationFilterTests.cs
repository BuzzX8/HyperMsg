using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class SerializationFilterTests
{
    private readonly SerializationFilter filter;
    private readonly IBuffer buffer;

    public SerializationFilterTests()
    {
        buffer = A.Fake<IBuffer>();
        filter = new();
    }

    [Fact]
    public void Serialize_Invokes_Registered_Serializer()
    {
        var message = Guid.NewGuid();
        var serializer = A.Fake<Action<IBufferWriter, Guid>>();
        filter.Register(serializer);

        filter.Serialize(buffer.Writer, message);

        A.CallTo(() => serializer.Invoke(buffer.Writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Serialize_Does_Not_Invokes_Deregistered_Serializer()
    {
        var message = Guid.NewGuid();
        var serializer = A.Fake<Action<IBufferWriter, Guid>>();
        filter.Register(serializer);
        filter.Deregister<Guid>();

        filter.Serialize(buffer.Writer, message);

        A.CallTo(() => serializer.Invoke(buffer.Writer, message)).MustNotHaveHappened();
    }
}
