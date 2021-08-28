using HyperMsg.Messages;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class SenderExtensions
    {
        #region Basic extensions

        public static void SendMessage<THeader, TBody>(this ISender sender, THeader header, TBody body) =>
            sender.Send(new Message<THeader, TBody>(header, body));

        public static Task SendMessageAsync<THeader, TBody>(this ISender sender, THeader header, TBody body, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new Message<THeader, TBody>(header, body), cancellationToken);

        public static void SendCommand<T>(this ISender sender, T command) =>
            sender.SendMessage(BasicMessageType.Command, command);

        public static Task SendCommandAsync<T>(this ISender sender, T command, CancellationToken cancellationToken = default) =>
            sender.SendMessageAsync(BasicMessageType.Command, command, cancellationToken);

        public static void SendEvent<T>(this ISender sender, T @event) =>
            sender.SendMessage(BasicMessageType.Event, @event);

        public static Task SendEventAsync<T>(this ISender sender, T @event, CancellationToken cancellationToken = default) =>
            sender.SendMessageAsync(BasicMessageType.Event, @event, cancellationToken);

        #endregion

        #region Buffer extensions

        /// <summary>
        /// Sends binary data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToTransmitBuffer(this ISender messageSender, ReadOnlyMemory<byte> data, bool invokeBufferHandler = true) => messageSender.SendToBuffer(CoreTopicType.Transmit, data, invokeBufferHandler);

        /// <summary>
        /// Sends binary data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync(this ISender messageSender, ReadOnlyMemory<byte> data, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(CoreTopicType.Transmit, data, invokeBufferHandler, cancellationToken);

        /// <summary>
        /// Sends stream data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToTransmitBuffer(this ISender messageSender, Stream stream, bool invokeBufferHandler = true) => messageSender.SendToBuffer(CoreTopicType.Transmit, stream, invokeBufferHandler);

        /// <summary>
        /// Sends stream data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync(this ISender messageSender, Stream stream, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(CoreTopicType.Transmit, stream, invokeBufferHandler, cancellationToken);

        /// <summary>
        /// Sends binary data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToReceiveBuffer(this ISender messageSender, ReadOnlyMemory<byte> data, bool invokeBufferHandler = true) => messageSender.SendToBuffer(CoreTopicType.Receive, data, invokeBufferHandler);

        /// <summary>
        /// Sends binary data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        public static Task SendToReceiveBufferAsync(this ISender messageSender, ReadOnlyMemory<byte> data, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(CoreTopicType.Receive, data, invokeBufferHandler, cancellationToken);

        /// <summary>
        /// Sends stream data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToReceiveBuffer(this ISender messageSender, Stream stream, bool invokeBufferHandler = true) => messageSender.SendToBuffer(CoreTopicType.Receive, stream, invokeBufferHandler);

        /// <summary>
        /// Sends stream data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="invokeBufferHandler">Indicates whatever buffer handler should be invoked or not.</param>
        public static Task SendToReceiveBufferAsync(this ISender messageSender, Stream stream, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(CoreTopicType.Receive, stream, invokeBufferHandler, cancellationToken);

        public static void SendToReceiveBuffer(this ISender messageSender, Action<IBufferWriter> writeAction, bool invokeBufferHandler = true) =>
            messageSender.SendToBuffer(CoreTopicType.Receive, writeAction, invokeBufferHandler);

        public static Task SendToReceiveBufferAsync(this ISender messageSender, AsyncAction<IBufferWriter> writeAction, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(CoreTopicType.Receive, writeAction, invokeBufferHandler, cancellationToken);

        public static Task SendToReceiveBufferAsync(this ISender messageSender, AsyncAction<IBufferWriter<byte>> writeAction, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(CoreTopicType.Receive, writeAction, invokeBufferHandler, cancellationToken);

        public static Task SendToReceiveBufferAsync(this ISender messageSender, Action<IBufferWriter> writeAction, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(CoreTopicType.Receive, writeAction, invokeBufferHandler, cancellationToken);

        public static void SendToReceiveBuffer(this ISender messageSender, Action<IBufferWriter<byte>> writeAction, bool invokeBufferHandler = true) =>
            messageSender.SendToBuffer(CoreTopicType.Receive, writeAction, invokeBufferHandler);

        public static Task SendToReceiveBufferAsync(this ISender messageSender, Action<IBufferWriter<byte>> writeAction, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(CoreTopicType.Receive, writeAction, invokeBufferHandler, cancellationToken);

        internal static void SendToBuffer<T>(this ISender messageSender, CoreTopicType topicType, T message, bool invokeBufferHandler = true) => 
            messageSender.Send<BufferServiceAction>(service => service.WriteToBuffer(topicType, message, invokeBufferHandler));

        internal static void SendToBuffer(this ISender messageSender, CoreTopicType topicType, Action<IBufferWriter> writeAction, bool invokeBufferHandler = true) =>
            messageSender.Send<BufferServiceAction>(service => service.WriteToBuffer(topicType, writeAction, invokeBufferHandler));

        internal static void SendToBuffer(this ISender messageSender, CoreTopicType topicType, Action<IBufferWriter<byte>> writeAction, bool invokeBufferHandler = true) =>
            messageSender.Send<BufferServiceAction>(service => service.WriteToBuffer(topicType, writeAction, invokeBufferHandler));

        internal static Task SendToBufferAsync(this ISender messageSender, CoreTopicType topicType, AsyncAction<IBufferWriter> writeAction, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync<BufferServiceAsyncAction>((service, token) => service.WriteToBufferAsync(topicType, writeAction, invokeBufferHandler, token), cancellationToken);

        internal static Task SendToBufferAsync(this ISender messageSender, CoreTopicType topicType, AsyncAction<IBufferWriter<byte>> writeAction, bool invokeBufferHandler = true, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync<BufferServiceAsyncAction>((service, token) => service.WriteToBufferAsync(topicType, writeAction, invokeBufferHandler, token), cancellationToken);

        internal static Task SendToBufferAsync<T>(this ISender messageSender, CoreTopicType topicType, T message, bool invokeBufferHandler = true, CancellationToken _ = default)
        {
            messageSender.SendToBuffer(topicType, message, invokeBufferHandler);
            return Task.CompletedTask;
        }

        internal static Task SendToBufferAsync(this ISender messageSender, CoreTopicType topicType, Action<IBufferWriter> writeAction, bool invokeBufferHandler = true, CancellationToken _ = default)
        {
            messageSender.SendToBuffer(topicType, writeAction, invokeBufferHandler);
            return Task.CompletedTask;
        }

        internal static Task SendToBufferAsync(this ISender messageSender, CoreTopicType topicType, Action<IBufferWriter<byte>> writeAction, bool invokeBufferHandler = true, CancellationToken _ = default)
        {
            messageSender.SendToBuffer(topicType, writeAction, invokeBufferHandler);
            return Task.CompletedTask;
        }

        #endregion

        #region Topic extensions


        /// <summary>
        /// Sends message to receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToTransportTopic<T>(this ISender messageSender, T message) => messageSender.SendToTopic(CoreTopicType.Transport, message);

        /// <summary>
        /// Sends message to receive topic asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToTransportTopicAsync<T>(this ISender messageSender, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToTopicAsync(CoreTopicType.Transport, message, cancellationToken);

        /// <summary>
        /// Sends message to transmit topic.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToTransmitTopic<T>(this ISender messageSender, T message) => messageSender.SendToTopic(CoreTopicType.Transmit, message);

        /// <summary>
        /// Sends message to transmit topic asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToTransmitTopicAsync<T>(this ISender messageSender, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToTopicAsync(CoreTopicType.Transmit, message, cancellationToken);

        /// <summary>
        /// Sends message to receive topic.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToReceiveTopic<T>(this ISender messageSender, T message) => messageSender.SendToTopic(CoreTopicType.Receive, message);

        /// <summary>
        /// Sends message to receive topic asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToReceiveTopicAsync<T>(this ISender messageSender, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToTopicAsync(CoreTopicType.Receive, message, cancellationToken);

        /// <summary>
        /// Sends message to topic.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToTopic<T>(this ISender messageSender, object topicId, T message) => 
            messageSender.Send(new TopicMessage<T>(topicId, message, messageSender));

        /// <summary>
        /// Sends message to topic asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="topicId">Topic ID.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>        
        public static Task SendToTopicAsync<T>(this ISender messageSender, object topicId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new TopicMessage<T>(topicId, message, messageSender), cancellationToken);

        #endregion
    }
}