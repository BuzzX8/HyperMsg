using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BufferService : MessagingService
    {
        private readonly IBufferContext bufferContext;
        private readonly object transmittingBufferLock = new();
        private readonly object receivingBufferLock = new();

        public BufferService(IMessagingContext messagingContext, IBufferContext bufferContext) : base(messagingContext) => 
            this.bufferContext = bufferContext;

        protected override IEnumerable<IDisposable> GetRegistrationHandles()
        {
            yield return HandlersRegistry.RegisterMessageHandler<BasicMessageType, Action<IBuffer>>(HandleBufferAction);
            yield return HandlersRegistry.RegisterMessageHandler<BasicMessageType, AsyncAction<IBuffer>>(HandleBufferActionAsync);
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
    }
}