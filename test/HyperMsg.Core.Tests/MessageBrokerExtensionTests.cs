using FakeItEasy;
using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new();
        private readonly Guid data = Guid.NewGuid();
        private readonly Action<Guid> handler = A.Fake<Action<Guid>>();
        private readonly AsyncAction<Guid> asyncHandler = A.Fake<AsyncAction<Guid>>();

        [Fact]
        public void SendReceiveEvent_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterReceiveEventHandler(handler);
            broker.RegisterReceiveEventHandler(asyncHandler);

            broker.SendReceiveEvent(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public async Task SendReceiveEventAsync_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterReceiveEventHandler(handler);
            broker.RegisterReceiveEventHandler(asyncHandler);

            await broker.SendReceiveEventAsync(data, default);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Invokes_Handler_If_Predicate_Returns_True()
        {            
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => true, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Does_Not_Invokes_Handler_If_Predicate_Returns_False()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => false, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendSerializeCommand_Invokes_Handler_Registtered_By_RegisterSerializationHandler()
        {
            var handler = A.Fake<Action<IBufferWriter<byte>, Guid>>();
            var bufferWriter = A.Fake<IBufferWriter<byte>>();
            var message = Guid.NewGuid();

            broker.RegisterSerializationHandler(handler);
            broker.SendSerializeCommand(bufferWriter, message);

            A.CallTo(() => handler.Invoke(bufferWriter, message)).MustHaveHappened();
        }

        [Fact]
        public void SendToBufferCommand_Invokes_Handle_Method_Of_Registered_Handler()
        {
            var handler = A.Fake<IWriteToBufferCommandHandler>();
            var message = Guid.NewGuid();

            broker.RegisterWriteToBufferCommandHandler(handler);
            broker.SendToBuffer(BufferType.None, message);

            A.CallTo(() => handler.WriteToBuffer(BufferType.None, message, true)).MustHaveHappened();
        }
    }
}
