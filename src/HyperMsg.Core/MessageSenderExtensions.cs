using HyperMsg.Messages;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageSenderExtensions
    {
        #region Buffer extensions

        /// <summary>
        /// Sends binary data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToTransmitBuffer(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Transmit, data, flushBuffer);

        /// <summary>
        /// Sends binary data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(PipeType.Transmit, data, flushBuffer, cancellationToken);

        /// <summary>
        /// Sends stream data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToTransmitBuffer(this IMessageSender messageSender, Stream stream, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Transmit, stream, flushBuffer);

        /// <summary>
        /// Sends stream data to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync(this IMessageSender messageSender, Stream stream, bool flushBuffer = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(PipeType.Transmit, stream, flushBuffer, cancellationToken);

        /// <summary>
        /// Sends binary data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToReceiveBuffer(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Receive, data, flushBuffer);

        /// <summary>
        /// Sends binary data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="data">Data which should be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        public static Task SendToReceiveBufferAsync(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(PipeType.Receive, data, flushBuffer, cancellationToken);

        /// <summary>
        /// Sends stream data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        public static void SendToReceiveBuffer(this IMessageSender messageSender, Stream stream, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Receive, stream, flushBuffer);

        /// <summary>
        /// Sends stream data to receive buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="stream">Stream which content be written into buffer.</param>
        /// <param name="flushBuffer">Indicates whatever buffer handler should be invoked or not.</param>
        public static Task SendToReceiveBufferAsync(this IMessageSender messageSender, Stream stream, bool flushBuffer = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(PipeType.Receive, stream, flushBuffer, cancellationToken);
        
        internal static void SendToBuffer<T>(this IMessageSender messageSender, PipeType pipeType, T message, bool flushBuffer = true) => 
            messageSender.Send<BufferServiceAction>(service => service.WriteToBuffer(pipeType, message, flushBuffer));
        
        internal static Task SendToBufferAsync<T>(this IMessageSender messageSender, PipeType pipeType, T message, bool flushBuffer = true, CancellationToken _ = default)
        {
            messageSender.SendToBuffer(pipeType, message, flushBuffer);
            return Task.CompletedTask;
        }

        #endregion

        #region Pipe extensions

        /// <summary>
        /// Sends message to transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToTransmitPipe<T>(this IMessageSender messageSender, T message) => messageSender.SendToTransmitPipe(null, message);

        /// <summary>
        /// Sends message to transmit pipe.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>        
        /// <param name="portId">Pipe port ID</param>
        /// <param name="message">Message to send.</param>
        public static void SendToTransmitPipe<T>(this IMessageSender messageSender, object portId, T message) => messageSender.SendToPipe(PipeType.Transmit, portId, message);

        /// <summary>
        /// Sends message to transmit pipe asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToTransmitPipeAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendToTransmitPipeAsync(null, message, cancellationToken);

        /// <summary>
        /// Sends message to transmit pipe asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="portId">Pipe port ID</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToTransmitPipeAsync<T>(this IMessageSender messageSender, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(PipeType.Transmit, portId, message, cancellationToken);

        /// <summary>
        /// Sends message to receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToReceivePipe<T>(this IMessageSender messageSender, T message) => messageSender.SendToReceivePipe(null, message);

        /// <summary>
        /// Sends message to receive pipe.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>        
        /// <param name="portId">Pipe port ID</param>
        /// <param name="message">Message to send.</param>
        public static void SendToReceivePipe<T>(this IMessageSender messageSender, object portId, T message) => messageSender.SendToPipe(PipeType.Receive, portId, message);

        /// <summary>
        /// Sends message to receive pipe asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToReceivePipeAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToReceivePipeAsync(null, message, cancellationToken);

        /// <summary>
        /// Sends message to receive pipe asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>        
        /// <param name="portId">Pipe port ID</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToReceivePipeAsync<T>(this IMessageSender messageSender, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(PipeType.Receive, portId, message, cancellationToken);

        /// <summary>
        /// Sends message to pipe.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="message">Message to send.</param>
        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, T message) =>
            messageSender.SendToPipe(pipeId, null, message);

        /// <summary>
        /// Sends message to pipe asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(pipeId, null, message, cancellationToken);

        /// <summary>
        /// Sends message to pipe.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="portId">Pipe port ID</param>
        /// <param name="message">Message to send.</param>
        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, object portId, T message) => 
            messageSender.Send(new PipeMessage<T>(pipeId, portId, message, messageSender));

        /// <summary>
        /// Sends message to pipe asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of message to send.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="pipeId">Pipe ID.</param>
        /// <param name="portId">Pipe port ID</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>        
        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new PipeMessage<T>(pipeId, portId, message, messageSender), cancellationToken);

        #endregion
    }
}