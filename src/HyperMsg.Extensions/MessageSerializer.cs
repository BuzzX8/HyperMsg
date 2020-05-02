using System;
using System.Buffers;

namespace HyperMsg
{
    internal class MessageSerializer<T> : IDisposable
    {
        private readonly ISender messageSender;
        private readonly IBuffer transmittingBuffer;
        private readonly Action<IBufferWriter<byte>, T> serializer;
        private readonly IDisposable subscription;

        internal MessageSerializer(IMessagingContext context, IBuffer transmittingBuffer, Action<IBufferWriter<byte>, T> serializer)
        {
            messageSender = context.Sender;
            this.transmittingBuffer = transmittingBuffer;
            this.serializer = serializer;
            subscription = context.Observable.OnTransmit<T>(Handle);
        }

        private void Handle(T message)
        {
            serializer.Invoke(transmittingBuffer.Writer, message);
            messageSender.TransmitBufferData(transmittingBuffer);
        }

        public void Dispose() => subscription.Dispose();
    }
}
