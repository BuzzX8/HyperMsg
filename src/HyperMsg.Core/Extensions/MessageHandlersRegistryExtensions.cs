using System;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class MessageHandlersRegistryExtensions
    {
        public static IDisposable RegisterTransmitMessageCommandHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> messageHandler) => handlersRegistry.RegisterHandler<TransmitMessageCommand<T>>(m => messageHandler.Invoke(m.Message));

        public static IDisposable RegisterTransmitMessageCommandHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> messageHandler) => handlersRegistry.RegisterHandler<TransmitMessageCommand<T>>((m, t) => messageHandler.Invoke(m.Message, t));

        public static IDisposable RegisterMessageReceivedEventHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> messageHandler) => handlersRegistry.RegisterHandler<MessageReceivedEvent<T>>(m => messageHandler.Invoke(m.Message));

        public static IDisposable RegisterMessageReceivedEventHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> messageHandler) => handlersRegistry.RegisterHandler<MessageReceivedEvent<T>>((m, t) => messageHandler.Invoke(m.Message, t));

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

        public static IDisposable RegisterTransmitBufferDataCommandHandler(this IMessageHandlersRegistry handlersRegistry, Action<ReadOnlyMemory<byte>> bufferDataHandler)
        {
            return new CompositeDisposable(new[] 
            { 
                handlersRegistry.RegisterTransmitMessageCommandHandler(bufferDataHandler),
                handlersRegistry.RegisterTransmitMessageCommandHandler<byte[]>(data => bufferDataHandler.Invoke(new ReadOnlyMemory<byte>(data))),
                handlersRegistry.RegisterTransmitMessageCommandHandler<ArraySegment<byte>>(data => bufferDataHandler.Invoke(data.AsMemory())) 
            });
        }

        public static IDisposable RegisterTransmitBufferDataCommandHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<ReadOnlyMemory<byte>> bufferDataHandler)
        {
            return new CompositeDisposable(new[]
            {
                handlersRegistry.RegisterTransmitMessageCommandHandler(bufferDataHandler),
                handlersRegistry.RegisterTransmitMessageCommandHandler<byte[]>((data, token) => bufferDataHandler.Invoke(new ReadOnlyMemory<byte>(data), token)),
                handlersRegistry.RegisterTransmitMessageCommandHandler<ArraySegment<byte>>((data, token) => bufferDataHandler.Invoke(data.AsMemory(), token))
            });
        }

        public static IDisposable RegisterReceivingBufferUpdatedEventHandler(this IMessageHandlersRegistry handlersRegistry, Action<IBuffer> messageHandler) =>
            handlersRegistry.RegisterMessageReceivedEventHandler(messageHandler);

        public static IDisposable RegisterReceivingBufferUpdatedEventHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBuffer> messageHandler) =>
            handlersRegistry.RegisterMessageReceivedEventHandler(messageHandler);
    }

    internal class CompositeDisposable : IDisposable
    {
        private readonly IDisposable[] disposables;

        internal CompositeDisposable(IDisposable[] disposables) => this.disposables = disposables;

        public void Dispose()
        {
            foreach(var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
