using FakeItEasy;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessageTransceiverTests
    {
        private readonly Pipe pipe;
        private readonly IStream stream;
        private readonly ISerializer<Guid> serializer;
        private readonly IObserver<Guid> observer;
        private readonly MessageTransceiver<Guid> transceiver;

        private TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public MessageTransceiverTests()
        {
            pipe = new Pipe();
            stream = new PipeStream(pipe.Reader, pipe.Writer);
            serializer = new DelegateMessageSerializer<Guid>(DeserializeGuid, SerializeGuid);
            observer = A.Fake<IObserver<Guid>>();
            transceiver = new MessageTransceiver<Guid>(stream, serializer, observer);
        }

        [Fact]
        public void Receives_Sends_Received_Messages()
        {
            using (var disp = transceiver.Run())
            {
                var message = Guid.NewGuid();
                var @event = new ManualResetEventSlim();

                transceiver.OnNextMessage += (s, a) => @event.Set();
                transceiver.Write(message);
                var flush = transceiver.FlushAsync().Result;
                @event.Wait(waitTimeout);

                A.CallTo(() => observer.OnNext(message)).MustHaveHappened();
            }
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