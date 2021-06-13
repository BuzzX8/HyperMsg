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

            yield return this.RegisterTransmitPipeHandler<Memory<byte>>(memory => WriteToBuffer(PipeType.Transmit, memory));
            yield return this.RegisterTransmitPipeHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(PipeType.Transmit, memory));
            yield return this.RegisterTransmitPipeHandler<ArraySegment<byte>>(segment => WriteToBuffer(PipeType.Transmit, segment));
            yield return this.RegisterTransmitPipeHandler<byte[]>(array => WriteToBuffer(PipeType.Transmit, array));
            yield return this.RegisterTransmitPipeHandler<Stream>(stream => WriteToBuffer(PipeType.Transmit, stream));

            yield return this.RegisterReceivePipeHandler<Memory<byte>>(memory => WriteToBuffer(PipeType.Receive, memory));
            yield return this.RegisterReceivePipeHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(PipeType.Receive, memory));
            yield return this.RegisterReceivePipeHandler<ArraySegment<byte>>(segment => WriteToBuffer(PipeType.Receive, segment));
            yield return this.RegisterReceivePipeHandler<byte[]>(array => WriteToBuffer(PipeType.Receive, array));
            yield return this.RegisterReceivePipeHandler<Stream>(stream => WriteToBuffer(PipeType.Receive, stream));
        }

        internal void WriteToBuffer<T>(PipeType bufferType, T message, bool flushBuffer = true)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(bufferType);

            WriteToBuffer(bufferType, message, buffer, bufferLock, flushBuffer);
        }

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(PipeType bufferType)
        {
            return bufferType switch
            {
                PipeType.Receive => (bufferContext.ReceivingBuffer, receivingBufferLock),
                PipeType.Transmit => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {bufferType} does not supported"),
            };
        }

        private void WriteToBuffer<T>(PipeType bufferType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
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

        internal void FlushBuffer(PipeType bufferType)
        {
            (var buffer, _) = GetBufferWithLock(bufferType);
            this.SendToPipeAsync(bufferType, buffer.Reader);
        }
    }
}