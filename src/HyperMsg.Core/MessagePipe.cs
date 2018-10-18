using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagePipe<T> : IMessageBuffer<T>, IDisposable
    {
        private readonly Func<ReadOnlySequence<byte>, int> bufferReader;
        private readonly PipeMessageBuffer<T> buffer;
        private readonly PipeReaderListener listener;
        private readonly Pipe pipe;

        public MessagePipe(SerializeAction<T> serializer, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            pipe = new Pipe();
            buffer = new PipeMessageBuffer<T>(pipe.Writer, serializer);
            listener = new PipeReaderListener(pipe.Reader, bufferReader);
        }

        public Task<FlushResult> FlushAsync(CancellationToken token = default) => buffer.FlushAsync(token);

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
