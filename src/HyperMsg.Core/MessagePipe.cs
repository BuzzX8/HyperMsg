using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagePipe<T> : IMessageBuffer<T>, IDisposable
    {
        private readonly Action<IBufferWriter<byte>, T> serializer;
        private readonly Func<ReadOnlySequence<byte>, int> bufferReader;
        private readonly MessageBuffer<T> buffer;
        private readonly PipeReaderListener listener;
        private readonly Pipe pipe;

        public MessagePipe(Action<IBufferWriter<byte>, T> serializer, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            pipe = new Pipe();
            buffer = new MessageBuffer<T>(pipe.Writer, serializer);
            listener = new PipeReaderListener(pipe.Reader, bufferReader);
        }

        public Task<FlushResult> FlushAsync() => buffer.FlushAsync();

        public void Write(T message) => buffer.Write(message);

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
