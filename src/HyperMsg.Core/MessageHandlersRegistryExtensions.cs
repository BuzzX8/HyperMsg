using HyperMsg.Messages;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace HyperMsg
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

        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter<byte>, T> serializationHandler) =>
            handlersRegistry.RegisterHandler<SerializationCommand<T>>(command => serializationHandler.Invoke(command.BufferWriter, command.Message));

        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferWriter<byte>, T> serializationHandler) =>
            handlersRegistry.RegisterHandler<SerializationCommand<T>>((command, token) => serializationHandler.Invoke(command.BufferWriter, command.Message, token));

        public static IDisposable RegisterBufferActionRequestHandler(this IMessageHandlersRegistry handlersRegistry, Action<Action<IBuffer>, BufferType> requestHandler) => 
            handlersRegistry.RegisterHandler<BufferActionRequest>(request => requestHandler.Invoke(request.BufferAction, request.BufferType));

        public static IDisposable RegisterBufferActionRequestHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<Action<IBuffer>, BufferType> requestHandler) => 
            handlersRegistry.RegisterHandler<BufferActionRequest>((request, token) => requestHandler.Invoke(request.BufferAction, request.BufferType, token));

        public static IDisposable RegisterReadFromBufferCommandHandler(this IMessageHandlersRegistry handlersRegistry, Action<BufferType, BufferReader> handler) =>
            handlersRegistry.RegisterHandler<ReadFromBufferCommand>(command => handler.Invoke(command.BufferType, command.BufferReader));

        public static IDisposable RegisterReadFromBufferCommandHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<BufferType, BufferReader> handler) =>
            handlersRegistry.RegisterHandler<ReadFromBufferCommand>((command, token) => handler.Invoke(command.BufferType, command.BufferReader, token));

        public static IDisposable RegisterWriteToBufferCommandHandler(this IMessageHandlersRegistry handlersRegistry, IWriteToBufferCommandHandler commandHandler) => 
            handlersRegistry.RegisterHandler<WriteToBufferCommand>(command => command.WriteToBufferAction.Invoke(commandHandler));

        public static IDisposable RegisterBufferUpdateEventHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, Action handler)
        {
            return handlersRegistry.RegisterHandler<BufferUpdatedEvent>(@event =>
            {
                if (@event.BufferType != bufferType)
                {
                    return;
                }

                handler.Invoke();
            });
        }

        public static IDisposable RegisterFlushBufferCommandHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, BufferReader flushHandler)
        {
            return handlersRegistry.RegisterHandler<FlushBufferEvent>(message =>
            {
                if (message.BufferType != bufferType)
                {
                    return;
                }

                message.BufferReaderAction.Invoke(flushHandler);
            });
        }

        public static IDisposable RegisterReceivingBufferUpdatedEventHandler(this IMessageHandlersRegistry handlersRegistry, Action<IBuffer> messageHandler) =>
            handlersRegistry.RegisterMessageReceivedEventHandler(messageHandler);

        public static IDisposable RegisterReceivingBufferUpdatedEventHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBuffer> messageHandler) =>
            handlersRegistry.RegisterMessageReceivedEventHandler(messageHandler);
    }
}