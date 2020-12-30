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
            broker.OnTransmit(handler);
            broker.OnTransmit(asyncHandler);

            broker.Transmit(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public void Transmit_Sends_Message_To_Transmit_Handlers1()
        {
            broker.OnTransmit(handler);
            broker.OnTransmit(asyncHandler);

            broker.Transmit<object>(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public async Task TransmitAsync_Sends_Message_To_Transmit_Handlers()
        {
            broker.OnTransmit(handler);
            broker.OnTransmit(asyncHandler);

            await broker.TransmitAsync(data, default);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public void Received_Sends_Message_To_Transmit_Handlers()
        {
            broker.OnReceived(handler);
            broker.OnReceived(asyncHandler);

            broker.Received(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public async Task ReceivedAsync_Sends_Message_To_Transmit_Handlers()
        {
            broker.OnReceived(handler);
            broker.OnReceived(asyncHandler);

            await broker.ReceivedAsync(data, default);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }
    }
}
