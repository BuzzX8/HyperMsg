using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class MessageTransceiver<T> : ITransceiver<T, T>, IDisposable
    {
        private readonly IPipeWriter writer;
        private readonly IObservable<T> messageObserveble;
        private readonly ISerializer<T> serializer;

        public MessageTransceiver(ISerializer<T> serializer, IPipeWriter writer, IObservable<T> messageObserveble)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.messageObserveble = messageObserveble ?? throw new ArgumentNullException(nameof(messageObserveble));
        }        

        public IDisposable Run() => this;

        public void Send(T message)
        {
            serializer.Serialize(writer, message);
            writer.Flush();
        }

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            serializer.Serialize(writer, message);
            await writer.FlushAsync(token);
        }

        public IDisposable Subscribe(IObserver<T> observer) => messageObserveble.Subscribe(observer);

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
