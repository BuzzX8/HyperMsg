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

        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter, T> serializationHandler) =>
            handlersRegistry.RegisterHandler<SerializeCommand<T>>(command => serializationHandler.Invoke(command.BufferWriter, command.Message));

        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferWriter, T> serializationHandler) =>
            handlersRegistry.RegisterHandler<SerializeCommand<T>>((command, token) => serializationHandler.Invoke(command.BufferWriter, command.Message, token));

        public static IDisposable RegisterRequestHandler<TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TResponse> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TResponse>>(message => message.Response = requestHandler.Invoke());

        public static IDisposable RegisterRequestHandler<TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<CancellationToken, Task<TResponse>> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TResponse>>(async (message, token) => message.Response = await requestHandler.Invoke(token));

        public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TRequest, TResponse> requestHandler) => 
            handlersRegistry.RegisterHandler<RequestResponseMessage<TRequest, TResponse>>(message => message.Response = requestHandler.Invoke(message.Request));

        public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TRequest, CancellationToken, Task<TResponse>> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TRequest, TResponse>>(async (message, token) => message.Response = await requestHandler.Invoke(message.Request, token));

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, handler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, portId, handler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, handler);

        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, portId, handler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, handler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, portId, handler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, handler);

        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<T> handler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, portId, handler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, Action<T> handler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, handler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, AsyncAction<T> handler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, handler);

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, Action<T> handler)
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

                handler.Invoke(message.Message);
            });
        }

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, AsyncAction<T> handler)
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

                return handler.Invoke(message.Message, token);
            });
        }
    }
}