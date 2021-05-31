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

        public static IDisposable RegisterWriteToBufferCommandHandler(this IMessageHandlersRegistry handlersRegistry, IWriteToBufferCommandHandler commandHandler) => 
            handlersRegistry.RegisterHandler<SendToBufferCommand>(command => command.WriteToBufferAction.Invoke(commandHandler));

        public static IDisposable RegisterBufferFlushReader(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, BufferReader bufferReader)
        {
            return handlersRegistry.RegisterHandler<FlushBufferEvent>(message =>
            {
                if (message.BufferType != bufferType)
                {
                    return;
                }

                message.BufferReaderAction.Invoke(bufferReader);
            });
        }

        public static IDisposable RegisterBufferFlushReader(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, AsyncBufferReader bufferReader)
        {
            return handlersRegistry.RegisterHandler<FlushBufferEvent>(message =>
            {
                if (message.BufferType != bufferType)
                {
                    return;
                }

                message.AsyncBufferReaderAction.Invoke(bufferReader);
            });
        }

        public static IDisposable RegisterBufferFlushSegmentReader(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, BufferSegmentReader segmentReader)
        {
            return handlersRegistry.RegisterBufferFlushReader(bufferType, buffer =>
            {
                if (buffer.Length == 0)
                {
                    return 0;
                }

                if (buffer.IsSingleSegment)
                {
                    return segmentReader(buffer.First);
                }

                var enumerator = buffer.GetEnumerator();
                var bytesRead = 0;

                while (enumerator.MoveNext())
                {
                    bytesRead += segmentReader(enumerator.Current);
                }

                return bytesRead;
            });
        }

        public static IDisposable RegisterBufferFlushSegmentReader(this IMessageHandlersRegistry handlersRegistry, BufferType bufferType, AsyncBufferSegmentReader segmentReader)
        {
            return handlersRegistry.RegisterBufferFlushReader(bufferType, async (buffer, token) =>
            {
                if (buffer.Length == 0)
                {
                    return 0;
                }

                if (buffer.IsSingleSegment)
                {
                    return await segmentReader(buffer.First, token);
                }

                var enumerator = buffer.GetEnumerator();
                var bytesRead = 0;

                while (enumerator.MoveNext())
                {
                    bytesRead += await segmentReader(enumerator.Current, token);
                }

                return bytesRead;
            });
        }
    }
}