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
            yield return HandlersRegistry.RegisterHandler<BufferServiceAction>(action => action.Invoke(this));

            yield return HandlersRegistry.RegisterTransmitTopicHandler<Memory<byte>>(memory => WriteToBuffer(CoreTopicType.Transmit, memory));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(CoreTopicType.Transmit, memory));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<ArraySegment<byte>>(segment => WriteToBuffer(CoreTopicType.Transmit, segment));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<byte[]>(array => WriteToBuffer(CoreTopicType.Transmit, array));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<Stream>(stream => WriteToBuffer(CoreTopicType.Transmit, stream));
            yield return HandlersRegistry.RegisterTransmitTopicHandler<BufferWriteAction>(action => WriteToBuffer(CoreTopicType.Transmit, action));            
            yield return HandlersRegistry.RegisterTransmitTopicHandler<ByteBufferWriteAction>(action => WriteToBuffer(CoreTopicType.Transmit, action));

            yield return HandlersRegistry.RegisterReceiveTopicHandler<Memory<byte>>(memory => WriteToBuffer(CoreTopicType.Receive, memory));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(CoreTopicType.Receive, memory));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<ArraySegment<byte>>(segment => WriteToBuffer(CoreTopicType.Receive, segment));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<byte[]>(array => WriteToBuffer(CoreTopicType.Receive, array));
            yield return HandlersRegistry.RegisterReceiveTopicHandler<Stream>(stream => WriteToBuffer(CoreTopicType.Receive, stream));
        }

        internal void WriteToBuffer<T>(CoreTopicType bufferType, T message, bool flushBuffer = true)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(bufferType);

            WriteToBuffer(bufferType, message, buffer, bufferLock, flushBuffer);
        }

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(CoreTopicType bufferType)
        {
            return bufferType switch
            {
                CoreTopicType.Receive => (bufferContext.ReceivingBuffer, receivingBufferLock),
                CoreTopicType.Transmit => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {bufferType} does not supported"),
            };
        }

        private void WriteToBuffer<T>(CoreTopicType TopicType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
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
                        var adapter = GetBufferWriterAdapter(TopicType);
                        writeAction.Invoke(adapter);
                        break;

                    case BufferWriteAction writeAction:
                        writeAction.Invoke(writer);
                        break;

                    case ByteBufferWriteAction writeAction:
                        adapter = GetBufferWriterAdapter(TopicType);
                        writeAction.Invoke(adapter);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            if (flushBuffer)
            {
                FlushBuffer(TopicType);
            }
        }

        private static void WriteStream(IBufferWriter writer, Stream stream)
        {
            var buffer = writer.GetMemory();
            var bytesRead = stream.Read(buffer.Span);

            writer.Advance(bytesRead);
        }

        private void FlushBuffer(CoreTopicType bufferType)
        {
            (var buffer, _) = GetBufferWithLock(bufferType);
            Sender.SendToTopicAsync(bufferType, buffer.Reader);
        }

        private IBufferWriter<byte> GetBufferWriterAdapter(CoreTopicType TopicType)
        {
            return TopicType switch
            {
                CoreTopicType.Receive => receiveBufferWriter,
                CoreTopicType.Transmit => transmitBufferWriter,
                _ => throw new NotSupportedException(),
            };
        }
    }

    internal delegate void BufferWriteAction(IBufferWriter bufferWriter);

    internal delegate void ByteBufferWriteAction(IBufferWriter<byte> bufferWriter);

    internal delegate void BufferServiceAction(BufferService bufferService);
}