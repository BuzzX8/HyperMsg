using FakeItEasy;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageTransceiverTests
    {
        private readonly Pipe pipe;
        private readonly IPipe stream;
        private readonly ISerializer<Guid> serializer;
        private readonly IObserver<Guid> observer;
        private readonly MessageTransceiver<Guid> transceiver;

        private TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public MessageTransceiverTests()
        {
            pipe = new Pipe();
            stream = new PipeProxy(pipe.Reader, pipe.Writer);
            //serializer = new DelegateMessageSerializer<Guid>(DeserializeGuid, SerializeGuid);
            //observer = A.Fake<IObserver<Guid>>();
            //transceiver = new MessageTransceiver<Guid>(stream, serializer, observer);
        }

        [Fact]
        public void Receives_Sends_Received_Messages()
        {
            
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