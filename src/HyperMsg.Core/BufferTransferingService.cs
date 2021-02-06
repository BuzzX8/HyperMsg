using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BufferTransferingService : MessagingObject, IHostedService
    {
        private readonly IBuffer transmittingBuffer;

        internal BufferTransferingService(IMessagingContext messagingContext, IBuffer transmittingBuffer) : base(messagingContext) => this.transmittingBuffer = transmittingBuffer;

        public void AddTransmittingBufferSerializer<TMessage>(Action<IBufferWriter<byte>, TMessage> serializer)
        {
            RegisterTransmitHandler<TMessage>(async (message, token) =>
            {
                serializer.Invoke(transmittingBuffer.Writer, message);
                await TransmitAsync(transmittingBuffer, token);
            });
        }

        public void AddReceivingBufferDeserializer<TMessage>(Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer) => RegisterReceiveHandler<IBuffer>((buffer, token) => DeserializeAsync(buffer, deserializer, token));

        public void AddReceivingBufferReader(Func<ReadOnlySequence<byte>, int> bufferReader) => RegisterReceiveHandler<IBuffer>(b => ReadBuffer(b, bufferReader));

        public void AddReceivingBufferReader(Func<ReadOnlySequence<byte>, CancellationToken, Task<int>> bufferReader) => RegisterReceiveHandler<IBuffer>((b, t) => ReadBufferAsync(b, bufferReader, t));

        private void ReadBuffer(IBuffer buffer, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            var reading = buffer.Reader.Read();
            if (reading.Length == 0)
            {
                return;
            }

            var bytesRead = bufferReader.Invoke(reading);

            if (bytesRead == 0)
            {
                return;
            }

            buffer.Reader.Advance(bytesRead);
        }

        private async Task ReadBufferAsync(IBuffer buffer, Func<ReadOnlySequence<byte>, CancellationToken, Task<int>> bufferReader, CancellationToken cancellationToken)
        {
            var reading = buffer.Reader.Read();
            if (reading.Length == 0)
            {
                return;
            }

            var bytesRead = await bufferReader.Invoke(reading, cancellationToken);

            if (bytesRead == 0)
            {
                return;
            }

            buffer.Reader.Advance(bytesRead);
        }

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

            return ReceiveAsync(message, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
