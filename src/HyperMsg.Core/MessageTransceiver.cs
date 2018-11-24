using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageTransceiver<T>
    {
        private readonly IStream stream;
        private readonly IMessageSerializer<T> serializer;
        private readonly IObserver<T> observer;

        private PipeReaderListener listener;

        public MessageTransceiver(IStream stream, IMessageSerializer<T> serializer, IObserver<T> observer)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));

            listener = new PipeReaderListener(stream.Reader, ReadBuffer);
        }

        public IDisposable Run() => listener.Run();

        public void Write(T message)
        {
            serializer.Serialize(stream.Writer, message);
        }

        public async Task<FlushResult> FlushAsync(CancellationToken token = default)
        {
            return await stream.Writer.FlushAsync(token);
        }

        private int ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            (var message, int readed) = serializer.Deserialize(buffer);

            if (message != null)
            {
                observer.OnNext(message);
                OnNextMessage?.Invoke(this, EventArgs.Empty);
            }

            return readed;
        }

        public event EventHandler OnNextMessage;
    }
}
