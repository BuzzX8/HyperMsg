using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageTransceiver<T> : ITransceiver<T, T>
    {
        private readonly MessageBuffer<T> messageBuffer;
        private readonly MessageReceiver<T> messageReceiver;

        public MessageTransceiver(ISerializer<T> serializer, IStream stream)
        {
            //messageBuffer = new MessageBuffer<T>(null, null, serializer.Serialize);
            messageReceiver = new MessageReceiver<T>(serializer.Deserialize, new Memory<byte>(), stream.ReadAsync);
        }

        public void Send(T message) => SendAsync(message).GetAwaiter().GetResult();

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public T Receive() => messageReceiver.Receive();

        public Task<T> ReceiveAsync(CancellationToken token = default) => messageReceiver.ReceiveAsync(token);
    }
}
