using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class SerializationMediatorTests
{
    private readonly IBufferWriter writer;
    private readonly ISerializersRegistry registry;
    private readonly Action<IBufferWriter, Guid> serializer;
    private readonly SerializationMediator mediator;

    public SerializationMediatorTests()
    {
        writer = A.Fake<IBufferWriter>();
        registry = A.Fake<ISerializersRegistry>();
        serializer = A.Fake<Action<IBufferWriter, Guid>>();
        A.CallTo(() => registry.Get<Guid>()).Returns(serializer);
        A.CallTo(() => registry.Contains<Guid>()).Returns(true);
        mediator = new SerializationMediator(writer, registry);
    }

    [Fact]
    public void Send_Invokes_Registered_Serializer()
    {
        var message = Guid.NewGuid();

        mediator.Send(message);

        A.CallTo(() => serializer.Invoke(writer, message)).MustHaveHappened();
    }

    [Fact]
    public async Task SendAsync_Invokes_Registered_Serializer()
    {
        var message = Guid.NewGuid();

        await mediator.SendAsync(message, default);

        A.CallTo(() => serializer.Invoke(writer, message)).MustHaveHappened();
    }

    [Fact]
    public void Send_Throws_Exception_If_No_Registered_Serializers_Found()
    {
        var message = Guid.NewGuid().ToString();

        Assert.Throws<InvalidOperationException>(() => mediator.Send(message));
    }

    [Fact]
    public async Task SendAsync_Throws_Exception_If_No_Registered_Serializers_Found()
    {
        var message = Guid.NewGuid().ToString();

        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync(message, default));
    }
}
