using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
            yield return HandlersRegistry.RegisterHandler<BufferServiceAction>(action => action.Invoke(this));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<BufferServiceAction>(action => action.Invoke(this));
            yield return HandlersRegistry.RegisterHandler<BufferServiceAsyncAction>((action, token) => action.Invoke(this, token));

            yield return HandlersRegistry.RegisterTransmitTopicHandler<Memory<byte>>(memory => WriteToBuffer(CoreTopicType.Transmit, memory));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(CoreTopicType.Transmit, memory));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<ArraySegment<byte>>(segment => WriteToBuffer(CoreTopicType.Transmit, segment));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<byte[]>(array => WriteToBuffer(CoreTopicType.Transmit, array));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<Stream>(stream => WriteToBuffer(CoreTopicType.Transmit, stream));

            yield return HandlersRegistry.RegisterReceiveTopicHandler<Memory<byte>>(memory => WriteToBuffer(CoreTopicType.Receive, memory));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(CoreTopicType.Receive, memory));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<ArraySegment<byte>>(segment => WriteToBuffer(CoreTopicType.Receive, segment));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<byte[]>(array => WriteToBuffer(CoreTopicType.Receive, array));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<Stream>(stream => WriteToBuffer(CoreTopicType.Receive, stream));
        }

        internal void WriteToBuffer<T>(CoreTopicType topicType, T message, bool flushBuffer = true)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(topicType);

            WriteToBuffer(topicType, message, buffer, bufferLock, flushBuffer);
        }

        internal void WriteToBuffer(CoreTopicType topicType, Action<IBufferWriter> writeAction, bool flushBuffer = true)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(topicType);

            lock(bufferLock)
            {
                writeAction.Invoke(buffer.Writer);
                
                if (flushBuffer)
                {
                    FlushBuffer(topicType);
                }
            }
        }

        internal void WriteToBuffer(CoreTopicType topicType, Action<IBufferWriter<byte>> writeAction, bool flushBuffer = true)
        {
            (_, var bufferLock) = GetBufferWithLock(topicType);
            var bufferWriter = GetBufferWriterAdapter(topicType);

            lock (bufferLock)
            {
                writeAction.Invoke(bufferWriter);

                if (flushBuffer)
                {
                    FlushBuffer(topicType);
                }
            }
        }

        internal async Task WriteToBufferAsync(CoreTopicType topicType, AsyncAction<IBufferWriter> writeAction, bool flushBuffer = true, CancellationToken cancellationToken = default)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(topicType);
                        
            Monitor.Enter(bufferLock);

            try
            {
                await writeAction.Invoke(buffer.Writer, cancellationToken);

                if (flushBuffer)
                {
                    FlushBuffer(topicType);
                }
            }
            finally
            {
                Monitor.Exit(bufferLock);
            }
        }

        internal async Task WriteToBufferAsync(CoreTopicType topicType, AsyncAction<IBufferWriter<byte>> writeAction, bool flushBuffer = true, CancellationToken cancellationToken = default)
        {
            (var _, var bufferLock) = GetBufferWithLock(topicType);
            var bufferWriter = GetBufferWriterAdapter(topicType);

            Monitor.Enter(bufferWriter);

            try
            {
                await writeAction.Invoke(bufferWriter, cancellationToken);

                if (flushBuffer)
                {
                    FlushBuffer(topicType);
                }
            }
            finally
            {
                Monitor.Exit(bufferLock);
            }
        }

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(CoreTopicType topicType)
        {
            return topicType switch
            {
                CoreTopicType.Receive => (bufferContext.ReceivingBuffer, receivingBufferLock),
                CoreTopicType.Transmit => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {topicType} does not supported"),
            };
        }

        private void WriteToBuffer<T>(CoreTopicType topicType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
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
                        throw new NotSupportedException();
                }
            }

            if (flushBuffer)
            {
                FlushBuffer(topicType);
            }
        }

        private static void WriteStream(IBufferWriter writer, Stream stream)
        {
            var buffer = writer.GetMemory();
            var bytesRead = stream.Read(buffer.Span);

            writer.Advance(bytesRead);
        }

        private void FlushBuffer(CoreTopicType topicType)
        {
            (var buffer, _) = GetBufferWithLock(topicType);
            Sender.SendToTopicAsync(topicType, buffer.Reader);
        }

        private IBufferWriter<byte> GetBufferWriterAdapter(CoreTopicType topicType)
        {
            return topicType switch
            {
                CoreTopicType.Receive => receiveBufferWriter,
                CoreTopicType.Transmit => transmitBufferWriter,
                _ => throw new NotSupportedException(),
            };
        }
    }

    internal delegate void BufferServiceAction(BufferService bufferService);

    internal delegate Task BufferServiceAsyncAction(BufferService bufferService, CancellationToken cancellationToken);
}