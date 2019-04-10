using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageTransceiver<T> : ITransceiver<T, T>
    {
        private readonly MessageBuffer<T> messageBuffer;
        private readonly MessageReceiver<T> messageReceiver;

        public MessageTransceiver(ISerializer<T> serializer, Memory<byte> sendBuffer, Memory<byte> receiveBuffer, IStream stream)
        {
            messageBuffer = new MessageBuffer<T>(serializer.Serialize, sendBuffer, stream.WriteAsync);
            messageReceiver = new MessageReceiver<T>(serializer.Deserialize, receiveBuffer, stream.ReadAsync);
        }

        public void Send(T message) => messageBuffer.Send(message);

        public Task SendAsync(T message, CancellationToken token = default) => messageBuffer.SendAsync(message, token);

        public T Receive() => messageReceiver.Receive();

        public Task<T> ReceiveAsync(CancellationToken token = default) => messageReceiver.ReceiveAsync(token);
    }
}
