using HyperMsg.Messages;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageHandlersRegistryExtensions
    {
        /// <summary>
        /// Registers handler which will be invoked if predicate returns true for message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="predicate">Message predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked if predicate returns true.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, Action messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m =>
            {
                if (predicate.Invoke(m))
                {
                    messageHandler.Invoke();
                }
            });

        /// <summary>
        /// Registers handler which will be invoked if predicate returns true for message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="predicate">Message predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked if predicate returns true.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, Action<T> messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m =>
            {
                if (predicate.Invoke(m))
                {
                    messageHandler.Invoke(m);
                }
            });

        /// <summary>
        /// Registers handler which will be invoked if predicate returns true for message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="predicate">Message predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked if predicate returns true.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, AsyncAction messageHandler) =>
            handlersRegistry.RegisterHandler<T>((m, t) =>
            {
                if (predicate.Invoke(m))
                {
                    return messageHandler.Invoke(t);
                }

                return Task.CompletedTask;
            });

        /// <summary>
        /// Registers handler which will be invoked if predicate returns true for message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="predicate">Message predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked if predicate returns true.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, AsyncAction<T> messageHandler) =>
            handlersRegistry.RegisterHandler<T>((m, t) =>
            {
                if (predicate.Invoke(m))
                {
                    return messageHandler.Invoke(m, t);
                }

                return Task.CompletedTask;
            });

        /// <summary>
        /// Registers handler which will be invoked for equal messages.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="message">Message instance which Equals method will be used as predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked for messages equal to provided message instance.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, Action messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m => m.Equals(message), messageHandler);

        /// <summary>
        /// Registers handler which will be invoked for equal messages.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="message">Message instance which Equals method will be used as predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked for messages equal to provided message instance.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, Action<T> messageHandler) =>
            handlersRegistry.RegisterHandler(m => m.Equals(message), messageHandler);

        /// <summary>
        /// Registers handler which will be invoked for equal messages.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="message">Message instance which Equals method will be used as predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked for messages equal to provided message instance.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, AsyncAction messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m => m.Equals(message), messageHandler);

        /// <summary>
        /// Registers handler which will be invoked for equal messages.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="message">Message instance which Equals method will be used as predicate.</param>
        /// <param name="messageHandler">Message handler which will be invoked for messages equal to provided message instance.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, AsyncAction<T> messageHandler) =>
            handlersRegistry.RegisterHandler(m => m.Equals(message), messageHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into transmit buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitBufferHandler(this IMessageHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into transmit buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitBufferHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into receive buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveBufferHandler(this IMessageHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into receive buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveBufferHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(bufferHandler);

        /// <summary>
        /// Registers handler which should serialize specified type of message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="serializationHandler">Handler which should serialize message into buffer.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter, T> serializationHandler) => 
            handlersRegistry.RegisterTransmitPipeHandler<T>((sender, message) => sender.SendToTransmitPipe<BufferWriteAction>(writer => serializationHandler.Invoke(writer, message)));

        /// <summary>
        /// Registers handler which should serialize specified type of message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="serializationHandler">Handler which should serialize message into buffer.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter<byte>, T> serializationHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler<T>((sender, message) => sender.SendToTransmitPipe<ByteBufferWriteAction>(writer => serializationHandler.Invoke(writer, message)));

        #region Pipe extensions

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler<T>(portId, (_, message) => pipeHandler.Invoke(message));

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, portId, pipeHandler);

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterTransmitPipeHandler<T>(portId, (_, message, token) => pipeHandler.Invoke(message, token));

        /// <summary>
        /// Registers handler for transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Transmit, portId, pipeHandler);

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler<T>(portId, (_, message) => pipeHandler.Invoke(message));

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler<T>(portId, (_, message, token) => pipeHandler.Invoke(message, token));

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, portId, pipeHandler);

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterReceivePipeHandler(null, pipeHandler);

        /// <summary>
        /// Registers handler for receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceivePipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object portId, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(PipeType.Receive, portId, pipeHandler);

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, Action<T> pipeHandler) => 
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, Action<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, AsyncAction<IMessageSender, T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler(pipeId, null, pipeHandler);

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, Action<T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler<T>(pipeId, portId, (_, message) => pipeHandler.Invoke(message));

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterPipeHandler<T>(this IMessageHandlersRegistry handlersRegistry, object pipeId, object portId, AsyncAction<T> pipeHandler) =>
            handlersRegistry.RegisterPipeHandler<T>(pipeId, portId, (_, message, token) => pipeHandler.Invoke(message, token));

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
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

        /// <summary>
        /// Registers pipe handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="portId">Pipe port ID.</param>
        /// <param name="pipeHandler">Pipe handler.</param>
        /// <returns>Registration handle.</returns>
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