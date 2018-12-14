using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageTransceiver<T> : IMessageBuffer<T>
    {
        private readonly IStream stream;
        private readonly ISerializer<T> serializer;
        private readonly IObserver<T> observer;

        private MessageListener<T> listener;

        public MessageTransceiver(IStream stream, ISerializer<T> serializer, IObserver<T> observer)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));

            listener = new MessageListener<T>(stream.Reader, serializer.Deserialize, observer);            
        }

        public IDisposable Run() => listener.Run();

        public void Write(T message)
        {
            serializer.Serialize(stream.Writer, message);
        }

        public Task FlushAsync(CancellationToken token = default)
        {
            return stream.Writer.FlushAsync(token).AsTask();
        }

        private void OnMessageReceived() => OnNextMessage?.Invoke(this, EventArgs.Empty);

        public event EventHandler OnNextMessage;
    }
}
