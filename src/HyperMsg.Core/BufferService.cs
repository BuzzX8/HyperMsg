using System;
using System.Collections.Generic;
using System.IO;

namespace HyperMsg
{
    public class BufferService : MessagingService
    {
        private readonly IBufferContext bufferContext;
        private readonly object transmittingBufferLock = new();
        private readonly object receivingBufferLock = new();

        public BufferService(IMessagingContext messagingContext, IBufferContext bufferContext) : base(messagingContext) => this.bufferContext = bufferContext;

        protected override IEnumerable<IDisposable> GetAutoDisposables()
        {
            yield return this.RegisterRequestHandler(() => this);
        }

        internal void WriteToBuffer<T>(BufferType bufferType, T message, bool flushBuffer)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(bufferType);

            WriteToBuffer(bufferType, message, buffer, bufferLock, flushBuffer);
        }

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(BufferType bufferType)
        {
            return bufferType switch
            {
                BufferType.Receiving => (bufferContext.ReceivingBuffer, receivingBufferLock),
                BufferType.Transmitting => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {bufferType} does not supported"),
            };
        }

        private void WriteToBuffer<T>(BufferType bufferType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
        {
            var writer = buffer.Writer;

            lock (bufferLock)
            {
                switch (message)
                {
                    case Memory<byte> memory:
                        writer.Write(memory.Span);
                        break;

                    case ReadOnlyMemory<byte> memory:
                        writer.Write(memory.Span);
                        break;

                    case ArraySegment<byte> arraySegment:
                        writer.Write(arraySegment.AsSpan());
                        break;

                    case byte[] array:
                        writer.Write(array);
                        break;

                    case Stream stream:
                        WriteStream(writer, stream);
                        break;

                    default:
                        this.SendSerializeCommand(writer, message);
                        break;
                }
            }

            if (flushBuffer)
            {
                FlushBuffer(bufferType);
            }
        }

        private void WriteStream(IBufferWriter writer, Stream stream)
        {
            var buffer = writer.GetMemory();
            var bytesRead = stream.Read(buffer.Span);

            writer.Advance(bytesRead);
        }

        internal void FlushBuffer(BufferType bufferType)
        {
            (var buffer, _) = GetBufferWithLock(bufferType);
            this.SendToPipeAsync(bufferType, buffer.Reader);
        }
    }
}