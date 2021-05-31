﻿using HyperMsg.Messages;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageHandlersRegistryExtensions
    {
        public static IDisposable RegisterReceiveEventHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> messageHandler) => handlersRegistry.RegisterHandler<ReceiveEvent<T>>(m => messageHandler.Invoke(m.Message));

        public static IDisposable RegisterReceiveEventHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> messageHandler) => handlersRegistry.RegisterHandler<ReceiveEvent<T>>((m, t) => messageHandler.Invoke(m.Message, t));

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
            handlersRegistry.RegisterHandler<SerializeCommand<T>>(command => serializationHandler.Invoke(command.BufferWriter, command.Message));

        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferWriter<byte>, T> serializationHandler) =>
            handlersRegistry.RegisterHandler<SerializeCommand<T>>((command, token) => serializationHandler.Invoke(command.BufferWriter, command.Message, token));

        public static IDisposable RegisterBufferFlushHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, Action<IBufferReader<byte>> handler)
        {
            return handlersRegistry.RegisterHandler<FlushBufferEvent>(message =>
            {
                if (message.BufferType != bufferType)
                {
                    return;
                }

                handler.Invoke(message.BufferReader);
            });
        }

        public static IDisposable RegisterBufferFlushHandler(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, AsyncAction<IBufferReader<byte>> handler)
        {
            return handlersRegistry.RegisterHandler<FlushBufferEvent>(async (message, token) =>
            {
                if (message.BufferType != bufferType)
                {
                    return;
                }

                await handler.Invoke(message.BufferReader, token);
            });
        }

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
                    return;
                }

                var enumerator = data.GetEnumerator();                

                while (enumerator.MoveNext())
                {
                    bufferDataHandler(enumerator.Current);
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
                    return;
                }

                var enumerator = data.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    await bufferDataHandler(enumerator.Current, token);
                }

                reader.Advance((int)data.Length);
            });
        }
    }
}