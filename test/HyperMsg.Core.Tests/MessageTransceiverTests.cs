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
        private readonly ISerializer<Guid> serializer;
        private readonly Memory<byte> receiveBuffer;
        private readonly Memory<byte> sendBuffer;
        private readonly IStream stream;
        private readonly MessageTransceiver<Guid> transceiver;

        const int GuidSize = 16;

        public MessageTransceiverTests()
        {
            serializer = A.Fake<ISerializer<Guid>>();
            stream = A.Fake<IStream>();
            receiveBuffer = new Memory<byte>(new byte[100]);
            sendBuffer = new Memory<byte>(new byte[100]);

            transceiver = new MessageTransceiver<Guid>(serializer, sendBuffer, receiveBuffer, stream);
        }

        [Fact]
        public void Send_Writes_Serialized_Message_Into_Stream()
        {
            var message = Guid.NewGuid();

            transceiver.Send(message);

            A.CallTo(() => serializer.Serialize(A<IBufferWriter<byte>>._, message)).MustHaveHappened();
            A.CallTo(() => stream.WriteAsync(A<Memory<byte>>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Writes_Serialized_Message_Into_Stream()
        {
            var message = Guid.NewGuid();

            await transceiver.SendAsync(message);

            A.CallTo(() => serializer.Serialize(A<IBufferWriter<byte>>._, message)).MustHaveHappened();
            A.CallTo(() => stream.WriteAsync(A<Memory<byte>>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void Receive_Returns_Message_From_DeserializationResult()
        {
            var expectedMessage = Guid.NewGuid();
            A.CallTo(() => serializer.Deserialize(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(GuidSize, expectedMessage));
            A.CallTo(() => stream.ReadAsync(A<Memory<byte>>._, A<CancellationToken>._)).Returns(Task.FromResult(GuidSize));

            var actualMessage = transceiver.Receive();

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task ReceiveAsync_Returns_Message_From_DeserializationResult()
        {
            var expectedMessage = Guid.NewGuid();
            A.CallTo(() => serializer.Deserialize(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(GuidSize, expectedMessage));
            A.CallTo(() => stream.ReadAsync(A<Memory<byte>>._, A<CancellationToken>._)).Returns(Task.FromResult(GuidSize));

            var actualMessage = await transceiver.ReceiveAsync();

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}