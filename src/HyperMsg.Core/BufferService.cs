using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BufferService : MessagingService
    {
        private readonly IBufferContext bufferContext;
        private readonly object transmittingBufferLock = new();
        private readonly object receivingBufferLock = new();

        public BufferService(IMessagingContext messagingContext, IBufferContext bufferContext) : base(messagingContext)
        {
            this.bufferContext = bufferContext;
        }

        protected override IEnumerable<IDisposable> GetRegistrationHandles()
        {
            yield return HandlersRegistry.RegisterMessageHandler<BasicMessageType, Action<IBuffer>>(HandleBufferAction);
            yield return HandlersRegistry.RegisterMessageHandler<BasicMessageType, AsyncAction<IBuffer>>(HandleBufferActionAsync);

            // yield return HandlersRegistry.RegisterTransmitTopicHandler<Memory<byte>>(memory => WriteToBuffer(CoreTopicType.Transmit, memory));
            // yield return HandlersRegistry.RegisterTransmitTopicHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(CoreTopicType.Transmit, memory));
            // yield return HandlersRegistry.RegisterTransmitTopicHandler<ArraySegment<byte>>(segment => WriteToBuffer(CoreTopicType.Transmit, segment));
            // yield return HandlersRegistry.RegisterTransmitTopicHandler<byte[]>(array => WriteToBuffer(CoreTopicType.Transmit, array));
            // yield return HandlersRegistry.RegisterTransmitTopicHandler<Stream>(stream => WriteToBuffer(CoreTopicType.Transmit, stream));

            // yield return HandlersRegistry.RegisterReceiveTopicHandler<Memory<byte>>(memory => WriteToBuffer(CoreTopicType.Receive, memory));
            // yield return HandlersRegistry.RegisterReceiveTopicHandler<ReadOnlyMemory<byte>>(memory => WriteToBuffer(CoreTopicType.Receive, memory));
            // yield return HandlersRegistry.RegisterReceiveTopicHandler<ArraySegment<byte>>(segment => WriteToBuffer(CoreTopicType.Receive, segment));
            // yield return HandlersRegistry.RegisterReceiveTopicHandler<byte[]>(array => WriteToBuffer(CoreTopicType.Receive, array));
            // yield return HandlersRegistry.RegisterReceiveTopicHandler<Stream>(stream => WriteToBuffer(CoreTopicType.Receive, stream));
        }

        private void HandleBufferAction(BasicMessageType type, Action<IBuffer> bufferAction)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(type);

            lock(bufferLock)
            {
                bufferAction.Invoke(buffer);
            }
        }

        private async Task HandleBufferActionAsync(BasicMessageType type, AsyncAction<IBuffer> bufferAction, CancellationToken cancellationToken)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(type);
                        
            Monitor.Enter(bufferLock);

            try
            {
                await bufferAction.Invoke(buffer, cancellationToken);
            }
            finally
            {
                Monitor.Exit(bufferLock);
            }
        }

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(BasicMessageType topicType)
        {
            return topicType switch
            {
                BasicMessageType.Receive => (bufferContext.ReceivingBuffer, receivingBufferLock),
                BasicMessageType.Transmit => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {topicType} does not supported"),
            };
        }

        // private void WriteToBuffer<T>(BasicMessageType topicType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
        // {
        //     var writer = buffer.Writer;

        //     lock (bufferLock)
        //     {
        //         switch (message)
        //         {
        //             case Memory<byte> memory:
        //                 writer.Write(memory.Span);
        //                 break;

        //             case ReadOnlyMemory<byte> memory:
        //                 writer.Write(memory.Span);
        //                 break;

        //             case ArraySegment<byte> arraySegment:
        //                 writer.Write(arraySegment.AsSpan());
        //                 break;

        //             case byte[] array:
        //                 writer.Write(array);
        //                 break;

        //             case Stream stream:
        //                 WriteStream(writer, stream);
        //                 break;

        //             default:
        //                 throw new NotSupportedException();
        //         }
        //     }

        //     if (flushBuffer)
        //     {
        //         FlushBuffer(topicType);
        //     }
        // }

        private static void WriteStream(IBufferWriter writer, Stream stream)
        {
            var buffer = writer.GetMemory();
            var bytesRead = stream.Read(buffer.Span);

            writer.Advance(bytesRead);
        }

        private void FlushBuffer(BasicMessageType topicType)
        {
            (var buffer, _) = GetBufferWithLock(topicType);
            Sender.SendToTopicAsync(topicType, buffer.Reader);
        }
    }

    internal delegate void BufferAction(IBuffer buffer);

    internal delegate Task BufferAsyncAction(IBuffer buffer, CancellationToken cancellationToken);
}