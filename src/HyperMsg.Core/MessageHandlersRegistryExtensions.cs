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
            handlersRegistry.RegisterReceiveTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into transmit buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitBufferHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into receive buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveBufferHandler(this IMessageHandlersRegistry handlersRegistry, Action<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which will be invoked each when new data written into receive buffer.
        /// </summary>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="bufferHandler">Buffer handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveBufferHandler(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IBufferReader> bufferHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler(bufferHandler);

        /// <summary>
        /// Registers handler which should serialize specified type of message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="serializationHandler">Handler which should serialize message into buffer.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter, T> serializationHandler) => 
            handlersRegistry.RegisterTransmitTopicHandler<T>((sender, message) => sender.SendToTransmitTopic<BufferWriteAction>(writer => serializationHandler.Invoke(writer, message)));

        /// <summary>
        /// Registers handler which should serialize specified type of message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="serializationHandler">Handler which should serialize message into buffer.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterSerializationHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IBufferWriter<byte>, T> serializationHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler<T>((sender, message) => sender.SendToTransmitTopic<ByteBufferWriteAction>(writer => serializationHandler.Invoke(writer, message)));

        #region Topic extensions

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> TopicHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler<T>((_, message) => TopicHandler.Invoke(message));

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IMessageSender, T> TopicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Transmit, TopicHandler);

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> TopicHandler) =>
            handlersRegistry.RegisterTransmitTopicHandler<T>((_, message, token) => TopicHandler.Invoke(message, token));

        /// <summary>
        /// Registers handler for transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTransmitTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IMessageSender, T> TopicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Transmit, TopicHandler);

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> TopicHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler<T>((_, message) => TopicHandler.Invoke(message));

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> TopicHandler) =>
            handlersRegistry.RegisterReceiveTopicHandler<T>((_, message, token) => TopicHandler.Invoke(message, token));

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<IMessageSender, T> TopicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Receive, TopicHandler);

        /// <summary>
        /// Registers handler for receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="TopicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterReceiveTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<IMessageSender, T> TopicHandler) =>
            handlersRegistry.RegisterTopicHandler(CoreTopicType.Receive, TopicHandler);

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, object topicId, Action<T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler<T>(topicId, (_, message) => topicHandler.Invoke(message));

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, object topicId, AsyncAction<T> topicHandler) =>
            handlersRegistry.RegisterTopicHandler<T>(topicId, (_, message, token) => topicHandler.Invoke(message, token));

        /// <summary>
        /// Registers topic handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handlersRegistry">Message handlers registry.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="topicHandler">Topic handler.</param>
        /// <returns>Registration handle.</returns>
        public static IDisposable RegisterTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, object topicId, Action<IMessageSender, T> topicHandler)
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
        public static IDisposable RegisterTopicHandler<T>(this IMessageHandlersRegistry handlersRegistry, object topicId, AsyncAction<IMessageSender, T> topicHandler)
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