using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class SerializationService : MessagingObject, IHostedService
    {
        private readonly IBuffer transmittingBuffer;

        internal SerializationService(IMessagingContext messagingContext, IBuffer transmittingBuffer) : base(messagingContext) => this.transmittingBuffer = transmittingBuffer;

        public void AddSerializer<TMessage>(Action<IBufferWriter<byte>, TMessage> serializer)
        {
            AddTransmitter<TMessage>(async (message, token) =>
            {
                serializer.Invoke(transmittingBuffer.Writer, message);
                await TransmitAsync(transmittingBuffer, token);
            });
        }

        public void AddDeserializer<TMessage>(Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer) => AddReceiver<IBuffer>((buffer, token) => DeserializeAsync(buffer, deserializer, token));

        private Task DeserializeAsync<TMessage>(IBuffer buffer, Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer, CancellationToken cancellationToken)
        {
            var reading = buffer.Reader.Read();
            if (reading.Length == 0)
            {
                return Task.CompletedTask;
            }

            var (bytesConsumed, message) = deserializer.Invoke(reading);

            if (bytesConsumed == 0)
            {
                return Task.CompletedTask;
            }

            buffer.Reader.Advance(bytesConsumed);

            return ReceivedAsync(message, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
