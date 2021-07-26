using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace HyperMsg
{
    public class BufferService : MessagingService
    {
        private readonly IBufferContext bufferContext;
        private readonly IBufferWriter<byte> transmitBufferWriter;
        private readonly IBufferWriter<byte> receiveBufferWriter;
        private readonly object transmittingBufferLock = new();
        private readonly object receivingBufferLock = new();

        public BufferService(IMessagingContext messagingContext, IBufferContext bufferContext) : base(messagingContext)
        {
            this.bufferContext = bufferContext;
            transmitBufferWriter = new BufferWriterAdapter(bufferContext.TransmittingBuffer.Writer);
            receiveBufferWriter = new BufferWriterAdapter(bufferContext.ReceivingBuffer.Writer);
        }

        protected override IEnumerable<IDisposable> GetRegistrationHandles()
        {
            yield return RegisterHandler<BufferServiceAction>(action => action.Invoke(this));

            yield return this.RegisterTransmitPipeHandler<Memory<byte>>(memory => WriteToBuffer(PipeType.Transmit, memory));
            yield return this.RegisterTransmitPipeHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(PipeType.Transmit, memory));
            yield return this.RegisterTransmitPipeHandler<ArraySegment<byte>>(segment => WriteToBuffer(PipeType.Transmit, segment));
            yield return this.RegisterTransmitPipeHandler<byte[]>(array => WriteToBuffer(PipeType.Transmit, array));
            yield return this.RegisterTransmitPipeHandler<Stream>(stream => WriteToBuffer(PipeType.Transmit, stream));
            yield return this.RegisterTransmitPipeHandler<BufferWriteAction>(action => WriteToBuffer(PipeType.Transmit, action));            
            yield return this.RegisterTransmitPipeHandler<ByteBufferWriteAction>(action => WriteToBuffer(PipeType.Transmit, action));

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

        private void WriteToBuffer<T>(PipeType pipeType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
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

                    case Action<IBufferWriter> writeAction:
                        writeAction.Invoke(writer);
                        break;

                    case Action<IBufferWriter<byte>> writeAction:
                        var adapter = GetBufferWriterAdapter(pipeType);
                        writeAction.Invoke(adapter);
                        break;

                    case BufferWriteAction writeAction:
                        writeAction.Invoke(writer);
                        break;

                    case ByteBufferWriteAction writeAction:
                        adapter = GetBufferWriterAdapter(pipeType);
                        writeAction.Invoke(adapter);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            if (flushBuffer)
            {
                FlushBuffer(pipeType);
            }
        }

        private void WriteStream(IBufferWriter writer, Stream stream)
        {
            var buffer = writer.GetMemory();
            var bytesRead = stream.Read(buffer.Span);

            writer.Advance(bytesRead);
        }

        private void FlushBuffer(PipeType bufferType)
        {
            (var buffer, _) = GetBufferWithLock(bufferType);
            this.SendToPipeAsync(bufferType, buffer.Reader);
        }

        private IBufferWriter<byte> GetBufferWriterAdapter(PipeType pipeType)
        {
            return pipeType switch
            {
                PipeType.Receive => receiveBufferWriter,
                PipeType.Transmit => transmitBufferWriter,
                _ => throw new NotSupportedException(),
            };
        }
    }

    internal delegate void BufferWriteAction(IBufferWriter bufferWriter);

    internal delegate void ByteBufferWriteAction(IBufferWriter<byte> bufferWriter);

    internal delegate void BufferServiceAction(BufferService bufferService);
}