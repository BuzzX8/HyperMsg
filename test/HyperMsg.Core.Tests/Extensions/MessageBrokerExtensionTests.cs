using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Extensions
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new MessageBroker();
        private readonly Guid data = Guid.NewGuid();
        private readonly Action<Guid> handler = A.Fake<Action<Guid>>();
        private readonly AsyncAction<Guid> asyncHandler = A.Fake<AsyncAction<Guid>>();

        [Fact]
        public void Transmit_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterTransmitHandler(handler);
            broker.RegisterTransmitHandler(asyncHandler);

            broker.Transmit(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public async Task TransmitAsync_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterTransmitHandler(handler);
            broker.RegisterTransmitHandler(asyncHandler);

            await broker.TransmitAsync(data, default);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public void Receive_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterReceiveHandler(handler);
            broker.RegisterReceiveHandler(asyncHandler);

            broker.Receive(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public async Task ReceiveAsync_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterReceiveHandler(handler);
            broker.RegisterReceiveHandler(asyncHandler);

            await broker.ReceiveAsync(data, default);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_()
        {            
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => true, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler__()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => false, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
        }
    }
}
