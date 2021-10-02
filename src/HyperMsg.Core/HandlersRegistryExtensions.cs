using System;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class HandlersRegistryExtensions
    {
        #region Buffer extensions

        public static IDisposable RegisterTransmitBufferHandler(this IHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Transmit, bufferHandler);
        
        public static IDisposable RegisterTransmitBufferHandler(this IHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Transmit, bufferHandler);
        
        public static IDisposable RegisterReceiveBufferHandler(this IHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Receive, bufferHandler);
        
        public static IDisposable RegisterReceiveBufferHandler(this IHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Receive, bufferHandler);

        internal static IDisposable RegisterBufferHandler(this IHandlersRegistry handlersRegistry, BufferType bufferType, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterHandler<BufferUpdatedEvent>(command => 
            {
                if (command.BufferType != bufferType)
                {
                    return;
                }

                bufferHandler.Invoke(command.BufferReader);
            });
        
        internal static IDisposable RegisterBufferHandler(this IHandlersRegistry handlersRegistry, BufferType bufferType, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterHandler<BufferUpdatedEvent>((command, token) => 
            {
                if (command.BufferType != bufferType)
                {
                    return Task.CompletedTask;
                }

                return bufferHandler.Invoke(command.BufferReader, token);
            });

        #endregion
    }
}