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

        public static IDisposable RegisterBufferFlushHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, Action<IBufferReader> handler) => 
            handlersRegistry.RegisterPipeHandler(bufferType, typeof(BufferService), handler);

        public static IDisposable RegisterBufferFlushHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, AsyncAction<IBufferReader> handler) => 
            handlersRegistry.RegisterPipeHandler(bufferType, typeof(BufferService), handler);

        public static IDisposable RegisterBufferFlushDataHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, Action<ReadOnlyMemory<byte>> bufferDataHandler)
        {
            return handlersRegistry.RegisterBufferFlushHandler(bufferType, reader =>
            {
                var data = reader.Read();

                if (data.Length == 0)
                {
                    return;
                }

                if (data.IsSingleSegment)
                {
                    bufferDataHandler(data.First);
                }
                else
                {
                    var enumerator = data.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        bufferDataHandler(enumerator.Current);
                    }
                }

                reader.Advance((int)data.Length);
            });
        }

        public static IDisposable RegisterBufferFlushDataHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, AsyncAction<ReadOnlyMemory<byte>> bufferDataHandler)
        {
            return handlersRegistry.RegisterBufferFlushHandler(bufferType, async (reader, token) =>
            {
                var data = reader.Read();

                if (data.Length == 0)
                {
                    return;
                }

                if (data.IsSingleSegment)
                {
                    await bufferDataHandler(data.First, token);
                }
                else
                {
                    var enumerator = data.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        await bufferDataHandler(enumerator.Current, token);
                    }
                }

                reader.Advance((int)data.Length);
            });
        }

        public static IDisposable RegisterRequestHandler<TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TResponse> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TResponse>>(message => message.Response = requestHandler.Invoke());

        public static IDisposable RegisterRequestHandler<TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<CancellationToken, Task<TResponse>> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TResponse>>(async (message, token) => message.Response = await requestHandler.Invoke(token));

        public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TRequest, TResponse> requestHandler) => 
            handlersRegistry.RegisterHandler<RequestResponseMessage<TRequest, TResponse>>(message => message.Response = requestHandler.Invoke(message.Request));

        public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IMessageHandlersRegistry handlersRegistry, Func<TRequest, CancellationToken, Task<TResponse>> requestHandler) =>
            handlersRegistry.RegisterHandler<RequestResponseMessage<TRequest, TResponse>>(async (message, token) => message.Response = await requestHandler.Invoke(message.Request, token));

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, Action<T> filter)
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

                filter.Invoke(message.Message);
            });
        }

        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, AsyncAction<T> filter)
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

                return filter.Invoke(message.Message, token);
            });
        }

        public static Task<T> WaitMessage<T>(this IMessageHandlersRegistry handlersRegistry, CancellationToken cancellationToken = default) => 
            handlersRegistry.WaitMessage<T>(_ => true, cancellationToken);

        public static Task<T> WaitMessage<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> messagePredicate, CancellationToken cancellationToken = default)
        {
            return WaitMessage<T>(completionSource => handlersRegistry.RegisterHandler<T>((message, token) =>
            {
                return Task.Run(() => Task.FromResult(messagePredicate.Invoke(message)))
                    .ContinueWith(completed =>
                    {
                        if (completed.IsCompletedSuccessfully && completed.Result)
                        {
                            completionSource.SetResult(message);
                        }

                        if (completed.IsFaulted)
                        {
                            completionSource.SetException(completed.Exception);
                        }
                    });
            }), cancellationToken);
        }

        internal static async Task<T> WaitMessage<T>(Func<TaskCompletionSource<T>, IDisposable> messageSubscriber, CancellationToken cancellationToken = default)
        {
            var completionSource = new TaskCompletionSource<T>();
            using var _ = cancellationToken.Register(() => completionSource.SetCanceled());
            using var __ = messageSubscriber.Invoke(completionSource);

            return await completionSource.Task;
        }
    }
}