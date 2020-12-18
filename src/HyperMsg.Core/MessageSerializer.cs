using System;
using System.Buffers;

namespace HyperMsg.Extensions
{
    internal class MessageSerializer<T>
    {
        private readonly IBuffer transmittingBuffer;
        private readonly Action<IBufferWriter<byte>, T> serializer;

        internal MessageSerializer(IMessagingContext context, IBuffer transmittingBuffer, Action<IBufferWriter<byte>, T> serializer)
        {
            this.transmittingBuffer = transmittingBuffer;
            this.serializer = serializer;
            //RegisterTransmitHandler<T>(Handle);
        }

        private void Handle(T message)
        {
            serializer.Invoke(transmittingBuffer.Writer, message);
            //Sender.TransmitBufferData(transmittingBuffer);
        }
    }
}
