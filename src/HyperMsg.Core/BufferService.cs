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
            yield return HandlersRegistry.RegisterHandler<BufferActionRequest>(HandleBufferAction);
            yield return HandlersRegistry.RegisterHandler<BufferAsyncActionRequest>(HandleBufferActionAsync);
            yield return HandlersRegistry.RegisterHandler<InvokeBufferHandlersCommand>(HandleBufferRequestAsync);
        }

        private void HandleBufferAction(BufferActionRequest request)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(request.BufferType);

            lock(bufferLock)
            {
                request.BufferAction.Invoke(buffer);
            }
        }

        private async Task HandleBufferActionAsync(BufferAsyncActionRequest request, CancellationToken cancellationToken)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(request.BufferType);
                        
            Monitor.Enter(bufferLock);

            try
            {
                await request.BufferAction.Invoke(buffer, cancellationToken);
            }
            finally
            {
                Monitor.Exit(bufferLock);
            }
        }

        private Task HandleBufferRequestAsync(InvokeBufferHandlersCommand command, CancellationToken cancellationToken) => 
            HandleBufferActionAsync(new BufferAsyncActionRequest(command.BufferType, (buffer, token) => Sender.SendAsync(new HandleBufferCommand(command.BufferType, buffer), token)), cancellationToken);

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(BufferType type)
        {
            return type switch
            {
                BufferType.Receive => (bufferContext.ReceivingBuffer, receivingBufferLock),
                BufferType.Transmit => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {type} does not supported"),
            };
        }
    }

    internal struct BufferActionRequest
    {
        public BufferActionRequest(BufferType bufferType, Action<IBuffer> bufferAction) =>
            (BufferType, BufferAction) = (bufferType, bufferAction);

        public BufferType BufferType { get; }

        public Action<IBuffer> BufferAction { get; }
    }
    
    internal struct BufferAsyncActionRequest
    {
        public BufferAsyncActionRequest(BufferType bufferType, AsyncAction<IBuffer> bufferAction) =>
            (BufferType, BufferAction) = (bufferType, bufferAction);

        public BufferType BufferType { get; }

        public AsyncAction<IBuffer> BufferAction { get; }
    }

    internal struct InvokeBufferHandlersCommand
    {
        public InvokeBufferHandlersCommand(BufferType bufferType) =>
            BufferType = bufferType;

        public BufferType BufferType { get; }
    }

    internal struct HandleBufferCommand
    {
        public HandleBufferCommand(BufferType bufferType, IBuffer buffer) => (BufferType, Buffer) = (bufferType, buffer);

        public IBuffer Buffer { get; }

        public BufferType BufferType { get; }
    }

    internal enum BufferType
    {
        Receive,
        Transmit
    }
}