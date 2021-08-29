using HyperMsg.Messages;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class HandlersRegistryExtensions
    {
        #region Basic extensions
        
        public static IDisposable RegisterMessageHandler<THeader, TBody>(this IHandlersRegistry handlersRegistry, Action<THeader, TBody> messageHandler) =>
            handlersRegistry.RegisterHandler<Message<THeader, TBody>>(m => messageHandler.Invoke(m.Header, m.Body));

        public static IDisposable RegisterMessageHandler<THeader, TBody>(this IHandlersRegistry handlersRegistry, AsyncAction<THeader, TBody> messageHandler) =>
            handlersRegistry.RegisterHandler<Message<THeader, TBody>>((m, t) => messageHandler.Invoke(m.Header, m.Body, t));

        public static IDisposable RegisterCommandHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> commandHandler) =>
            handlersRegistry.RegisterMessageHandler<BasicMessageType, T>((type, body) =>
            {
                if (!Equals(type, BasicMessageType.Command))
                {
                    return;
                }

                commandHandler.Invoke(body);
            });

        public static IDisposable RegisterCommandHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> commandHandler) =>
            handlersRegistry.RegisterMessageHandler<BasicMessageType, T>((type, body, token) =>
            {
                if (!Equals(type, BasicMessageType.Command))
                {
                    return Task.CompletedTask;
                }

                return commandHandler.Invoke(body, token);
            });

        public static IDisposable RegisterEventHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> eventHandler) =>
            handlersRegistry.RegisterMessageHandler<BasicMessageType, T>((type, body) =>
            {
                if (!Equals(type, BasicMessageType.Event))
                {
                    return;
                }

                eventHandler.Invoke(body);
            });

        public static IDisposable RegisterEventHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> eventHandler) =>
            handlersRegistry.RegisterMessageHandler<BasicMessageType, T>((type, body, token) =>
            {
                if (!Equals(type, BasicMessageType.Event))
                {
                    return Task.CompletedTask;
                }

                return eventHandler.Invoke(body, token);
            });

        #endregion

        #region Transfering extensions

        public static IDisposable RegisterTransmitCommandHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> commandHandler) =>
            handlersRegistry.RegisterCommandHandler<Message<BasicMessageType, T>>(message =>
            {
                if (!Equals(message.Header, BasicMessageType.Transmit))
                {
                    return;
                }

                commandHandler.Invoke(message.Body);
            });

        public static IDisposable RegisterTransmitCommandHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> commandHandler) =>
            handlersRegistry.RegisterCommandHandler<Message<BasicMessageType, T>>((message, token) =>
            {
                if (!Equals(message.Header, BasicMessageType.Transmit))
                {
                    return Task.CompletedTask;
                }

                return commandHandler.Invoke(message.Body, token);
            });

        public static IDisposable RegisterReceiveEventHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> commandHandler) =>
            handlersRegistry.RegisterEventHandler<Message<BasicMessageType, T>>(message =>
            {
                if (!Equals(message.Header, BasicMessageType.Receive))
                {
                    return;
                }

                commandHandler.Invoke(message.Body);
            });

        public static IDisposable RegisterReceiveEventHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> commandHandler) =>
            handlersRegistry.RegisterEventHandler<Message<BasicMessageType, T>>((message, token) =>
            {
                if (!Equals(message.Header, BasicMessageType.Receive))
                {
                    return Task.CompletedTask;
                }

                return commandHandler.Invoke(message.Body, token);
            });

        #endregion

        #region Buffer extensions

        /// <summary>
        /// Registers handler which will be invoked each when new data written into transmit buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitBufferHandler(this IHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into transmit buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitBufferHandler(this IHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into receive buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveBufferHandler(this IHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into receive buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveBufferHandler(this IHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler(bufferHandler);

        #endregion

        #region Filter extensions

        public static IDisposable RegisterFilter<T>(this IHandlersRegistry handlersRegistry, Action<ISender, T> filterHandler) =>
            handlersRegistry.RegisterMessageHandler(filterHandler);
        
        public static IDisposable RegisterFilter<T>(this IHandlersRegistry handlersRegistry, AsyncAction<ISender, T> filterHandler) =>
            handlersRegistry.RegisterMessageHandler(filterHandler);

        #endregion

        #region Serialization extensions

        /// <summary>
        /// Registers handler which should serialize specified type of message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="serializationHandler">Handler which should serialize message into buffer.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterSerializationHandler<T>(this IHandlersRegistry handlersRegistry, Action<IBufferWriter, T> serializationHandler) => 
            handlersRegistry.RegisterTransmitTopicHandler<T>((sender, message) => sender.SendToTransmitTopic<BufferServiceAction>(service => SerializeMessage(service, CoreTopicType.Transmit, message, serializationHandler)));

        private static void SerializeMessage<T>(BufferService service, CoreTopicType topicType, T message, Action<IBufferWriter, T> serializationHandler) => 
            service.WriteToBuffer(topicType, writer => serializationHandler.Invoke(writer, message));

        /// <summary>
        /// Registers handler which should serialize specified type of message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="serializationHandler">Handler which should serialize message into buffer.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterSerializationHandler<T>(this IHandlersRegistry handlersRegistry, Action<IBufferWriter<byte>, T> serializationHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler<T>((sender, message) => sender.SendToTransmitTopic<BufferServiceAction>(service => SerializeMessage(service, CoreTopicType.Transmit, message, serializationHandler)));

        private static void SerializeMessage<T>(BufferService service, CoreTopicType topicType, T message, Action<IBufferWriter<byte>, T> serializationHandler) => 
            service.WriteToBuffer(topicType, writer => serializationHandler.Invoke(writer, message));

        #endregion

        #region Topic extensions

        /// <summary>
        /// Registers handler for transport topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransportTopicHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Transport, topicHandler);

        /// <summary>
        /// Registers handler for transport topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransportTopicHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Transport, topicHandler);

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> topicHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler<T>((_, message) => topicHandler.Invoke(message));

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IHandlersRegistry handlersRegistry, Action<ISender, T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Transmit, topicHandler);

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> topicHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler<T>((_, message, token) => topicHandler.Invoke(message, token));

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<ISender, T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Transmit, topicHandler);

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> topicHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler<T>((_, message) => topicHandler.Invoke(message));

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> topicHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler<T>((_, message, token) => topicHandler.Invoke(message, token));

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IHandlersRegistry handlersRegistry, Action<ISender, T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Receive, topicHandler);

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<ISender, T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Receive, topicHandler);

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IHandlersRegistry handlersRegistry, object topicId, Action<T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler<T>(topicId, (_, message) => topicHandler.Invoke(message));

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IHandlersRegistry handlersRegistry, object topicId, AsyncAction<T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler<T>(topicId, (_, message, token) => topicHandler.Invoke(message, token));

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IHandlersRegistry handlersRegistry, object topicId, Action<ISender, T> topicHandler)
        {
            return handlersRegistry.RegisterHandler<TopicMessage<T>>(message =>
            {
                if (!Equals(topicId, message.TopicId))
                {
                    return;
                }

                topicHandler.Invoke(message.MessageSender, message.Message);
            });
        }

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IHandlersRegistry handlersRegistry, object topicId, AsyncAction<ISender, T> topicHandler)
        {
            return handlersRegistry.RegisterHandler<TopicMessage<T>>((message, token) =>
            {
                if (!Equals(topicId, message.TopicId))
                {
                    return Task.CompletedTask;
                }

                return topicHandler.Invoke(message.MessageSender, message.Message, token);
            });
        }

        #endregion
    }
}