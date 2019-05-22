using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageTransceiverTests
    {        
        private readonly MessageTransceiver<Guid> transceiver;
        private readonly IReceiver<Guid> receiver;
        private readonly ISender<Guid> sender;

        private readonly CancellationToken cancellationToken;

        public MessageTransceiverTests()
        {
            receiver = A.Fake<IReceiver<Guid>>();
            sender = A.Fake<ISender<Guid>>();
            transceiver = new MessageTransceiver<Guid>(receiver, sender);

            cancellationToken = new CancellationToken();
        }

        [Fact]
        public void Send_Sends_Message_Via_Sender()
        {
            var message = Guid.NewGuid();

            transceiver.Send(message);

            A.CallTo(() => sender.Send(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Sends_Message_Via_Sender()
        {
            var message = Guid.NewGuid();

            await transceiver.SendAsync(message, cancellationToken);

            A.CallTo(() => sender.SendAsync(message, cancellationToken)).MustHaveHappened();
        }

        [Fact]
        public void Receive_Returns_Message_From_Receiver()
        {
            var expectedMessage = Guid.NewGuid();
            A.CallTo(() => receiver.Receive()).Returns(expectedMessage);

            var actualMessage = transceiver.Receive();

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task ReceiveAsync_Returns_Message_From_Receiver()
        {
            var expectedMessage = Guid.NewGuid();
            A.CallTo(() => receiver.ReceiveAsync(cancellationToken)).Returns(Task.FromResult(expectedMessage));

            var actualMessage = await transceiver.ReceiveAsync(cancellationToken);

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}