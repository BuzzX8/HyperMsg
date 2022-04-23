using FakeItEasy;
using Xunit;

namespace HyperMsg
{
    public class SerializersRegistryTests
    {
        private readonly MessageBroker broker = new();
        private readonly SerializersRegistry serializersRegistry;
        private readonly IBuffer buffer;

        public SerializersRegistryTests()
        {
            buffer = A.Fake<IBuffer>();
            serializersRegistry = new SerializersRegistry(broker, buffer);
        }

        [Fact]
        public void Dispatch_Invokes_Registered_Serializer()
        {
            var message = Guid.NewGuid();
            var serializer = A.Fake<Action<IBufferWriter, Guid>>();
            serializersRegistry.Register(serializer);

            broker.Dispatch(message);

            A.CallTo(() => serializer.Invoke(buffer.Writer, message)).MustHaveHappened();
        }

        [Fact]
        public void Dispatch_Does_Not_Invokes_Deregistered_Serializer()
        {
            var message = Guid.NewGuid();
            var serializer = A.Fake<Action<IBufferWriter, Guid>>();
            serializersRegistry.Register(serializer);
            serializersRegistry.Deregister<Guid>();

            broker.Dispatch(message);

            A.CallTo(() => serializer.Invoke(buffer.Writer, message)).MustNotHaveHappened();
        }

        [Fact]
        public void Dispatch_Rises_BufferUpdated_Event()
        {
            var message = Guid.NewGuid();
            var serializer = A.Fake<Action<IBufferWriter, Guid>>();
            var eventHandler = A.Fake<Action<IBuffer>>();
            serializersRegistry.Register(serializer);
            serializersRegistry.BufferUpdated += eventHandler;

            broker.Dispatch(message);

            A.CallTo(() => eventHandler.Invoke(buffer)).MustHaveHappened();
        }
    }
}
