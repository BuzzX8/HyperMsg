using FakeItEasy;
using Xunit;

namespace HyperMsg;

public class MessageBrokerTests
{
    private readonly MessageBroker broker = new();

    [Fact]
    public void Dispatch_Invokes_Registered_Handler()
    {
        var message = Guid.NewGuid();
        var handler = A.Fake<Action<Guid>>();
        broker.Register(handler);

        broker.Dispatch(message);

        A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
    }

    [Fact]
    public void Dispatch_Does_Not_Invokes_Registered_Handler()
    {
        var message = Guid.NewGuid().ToString();
        var handler = A.Fake<Action<Guid>>();
        broker.Register(handler);

        broker.Dispatch(message);

        A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
    }

    [Fact]
    public void Dispatch_Does_Not_Invokes_Deregistered_Handler()
    {
        var message = Guid.NewGuid().ToString();
        var handler = A.Fake<Action<Guid>>();
        broker.Register(handler);
        broker.Deregister(handler);

        broker.Dispatch(message);

        A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
    }
}
