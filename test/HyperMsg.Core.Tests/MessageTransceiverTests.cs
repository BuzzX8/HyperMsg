using FakeItEasy;
using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageTransceiverTests
    {        
        private readonly IPipe pipe;
        private readonly ISerializer<Guid> serializer;
        private readonly MessageTransceiver<Guid> transceiver;

        public MessageTransceiverTests()
        {
            pipe = A.Fake<IPipe>();
            serializer = A.Fake<ISerializer<Guid>>();
            transceiver = new MessageTransceiver<Guid>(serializer, pipe);
        }

        [Fact]
        public void Send_Serializes_Message()
        {
            var message = Guid.NewGuid();

            transceiver.Send(message);

            A.CallTo(() => serializer.Serialize(pipe.Writer, message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Serializes_Message()
        {
            var message = Guid.NewGuid();

            await transceiver.SendAsync(message);

            A.CallTo(() => serializer.Serialize(pipe.Writer, message)).MustHaveHappened();
        }

        private static (Guid, int) DeserializeGuid(ReadOnlySequence<byte> buffer)
        {
            var data = buffer.ToArray();
            return (new Guid(data), data.Length);
        }

        private static void SerializeGuid(IBufferWriter<byte> writer, Guid message)
        {
            writer.Write(message.ToByteArray());
        }
    }
}