using FakeItEasy;
using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageTransceiverTests
    {
        private readonly IMessageBuffer<Guid> messageBuffer;
        private readonly MessageTransceiver<Guid> transceiver;
        private readonly IObservable<Guid> observer;

        public MessageTransceiverTests()
        {
            messageBuffer = A.Fake<IMessageBuffer<Guid>>();
            observer = A.Fake<IObservable<Guid>>();
            transceiver = new MessageTransceiver<Guid>(messageBuffer, observer);
        }

        [Fact]
        public void Send_Serializes_Message()
        {
            var message = Guid.NewGuid();

            transceiver.Send(message);

            A.CallTo(() => messageBuffer.Write(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Serializes_Message()
        {
            var message = Guid.NewGuid();

            await transceiver.SendAsync(message);

            A.CallTo(() => messageBuffer.Write(message)).MustHaveHappened();
        }
    }
}