using HyperMsg.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageHandlersRegistryExtensions
    {
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, Action messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m =>
            {
                if (predicate.Invoke(m))
                {
                    messageHandler.Invoke();
                }
            });

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, Action<T> messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m =>
            {
                if (predicate.Invoke(m))
                {
                    messageHandler.Invoke(m);
                }
            });

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, AsyncAction messageHandler) =>
            handlersRegistry.RegisterHandler<T>((m, t) =>
            {
                if (predicate.Invoke(m))
                {
                    return messageHandler.Invoke(t);
                }

                return Task.CompletedTask;
            });

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, AsyncAction<T> messageHandler) =>
            handlersRegistry.RegisterHandler<T>((m, t) =>
            {
                if (predicate.Invoke(m))
                {
                    return messageHandler.Invoke(m, t);
                }

                return Task.CompletedTask;
            });

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, Action messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m => m.Equals(message), messageHandler);

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, Action<T> messageHandler) =>
            handlersRegistry.RegisterHandler(m => m.Equals(message), messageHandler);

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, AsyncAction messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m => m.Equals(message), messageHandler);

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, AsyncAction<T> messageHandler) =>
            handlersRegistry.RegisterHandler(m => m.Equals(message), messageHandler);

        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter, T> serializationHandler)
        {            
            return handlersRegistry.RegisterTransmitPipeHandler<T>((sender, message) => sender.SendToTransmitPipe<BufferWriteAction>(writer => serializationHandler.Invoke(writer, message)));
        }

        public static IDisposable RegisterRequestHandler<TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TResponse> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TResponse>>(message => message.Response = requestHandler.Invoke());

        public static IDisposable RegisterRequestHandler<TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<CancellationToken, Task<TResponse>> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TResponse>>(async (message, token) => message.Response = await requestHandler.Invoke(token));

        public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TRequest, TResponse> requestHandler) => 
            handlersRegistry.RegisterHandler<RequestResponseMessage<TRequest, TResponse>>(message => message.Response = requestHandler.Invoke(message.Request));

        public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TRequest, CancellationToken, Task<TResponse>> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TRequest, TResponse>>(async (message, token) => message.Response = await requestHandler.Invoke(message.Request, token));

        #region Pipe extensions

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(null, pipeHandler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<T> handler) =>
            handlersRegistry.RegisterTransmitPipeHandler<T>(portId, (_, message) => handler.Invoke(message));

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(null, pipeHandler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, portId, pipeHandler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> handler) =>
            handlersRegistry.RegisterTransmitPipeHandler(null, handler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler<T>(portId, (_, message, token) => pipeHandler.Invoke(message, token));

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, portId, pipeHandler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler<T>(portId, (_, message) => pipeHandler.Invoke(message));

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler<T>(portId, (_, message, token) => pipeHandler.Invoke(message, token));

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, portId, pipeHandler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, portId, pipeHandler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, Action<T> pipeHandler) => 
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, Action<T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler<T>(pipeId, portId, (_, message) => pipeHandler.Invoke(message));

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler<T>(pipeId, portId, (_, message, token) => pipeHandler.Invoke(message, token));

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, Action<IMessageSender, T> pipeHandler)
        {
            return handlersRegistry.RegisterHandler<PipeMessage<T>>(message =>
            {
                if (!Equals(pipeId, message.PipeId))
                {
                    return;
                }

                if (!Equals(portId, message.PortId))
                {
                    return;
                }                

                pipeHandler.Invoke(message.MessageSender, message.Message);
            });
        }

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, AsyncAction<IMessageSender, T> pipeHandler)
        {
            return handlersRegistry.RegisterHandler<PipeMessage<T>>((message, token) =>
            {
                if (!Equals(pipeId, message.PipeId))
                {
                    return Task.CompletedTask;
                }

                if (!Equals(portId, message.PortId))
                {
                    return Task.CompletedTask;
                }

                return pipeHandler.Invoke(message.MessageSender, message.Message, token);
            });
        }

        #endregion
    }
}