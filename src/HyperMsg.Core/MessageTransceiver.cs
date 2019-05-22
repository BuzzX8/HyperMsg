using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageTransceiver<T> : ITransceiver<T, T>
    {
        private readonly IReceiver<T> receiver;
        private readonly ISender<T> sender;

        public MessageTransceiver(IReceiver<T> receiver, ISender<T> sender)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public void Send(T message) => sender.Send(message);

        public Task SendAsync(T message, CancellationToken cancellationToken) => sender.SendAsync(message, cancellationToken);

        public T Receive() => receiver.Receive();

        public Task<T> ReceiveAsync(CancellationToken cancellationToken) => receiver.ReceiveAsync(cancellationToken);
    }
}
