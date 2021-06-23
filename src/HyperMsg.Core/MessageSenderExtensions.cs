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
        /// Sends message to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        public static void SendToTransmitBuffer(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Transmit, data, flushBuffer);

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(PipeType.Transmit, data, flushBuffer, cancellationToken);

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        public static void SendToTransmitBuffer(this IMessageSender messageSender, Stream stream, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Transmit, stream, flushBuffer);

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync(this IMessageSender messageSender, Stream stream, bool flushBuffer = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(PipeType.Transmit, stream, flushBuffer, cancellationToken);

        public static void SendToReceiveBuffer(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Receive, data, flushBuffer);

        public static Task SendToReceiveBufferAsync(this IMessageSender messageSender, ReadOnlyMemory<byte> data, bool flushBuffer = true, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(PipeType.Receive, data, flushBuffer, cancellationToken);

        public static void SendToReceiveBuffer(this IMessageSender messageSender, Stream stream, bool flushBuffer = true) => messageSender.SendToBuffer(PipeType.Receive, stream, flushBuffer);

        public static Task SendToReceiveBufferAsync(this IMessageSender messageSender, Stream stream, bool flushBuffer = true, CancellationToken cancellationToken = default) =>
            messageSender.SendToBufferAsync(PipeType.Receive, stream, flushBuffer, cancellationToken);

        /// <summary>
        /// Sends message to buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="pipeType">Buffer type.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="flushBuffer"></param>
        /// <returns></returns>
        public static void SendToBuffer<T>(this IMessageSender messageSender, PipeType pipeType, T message, bool flushBuffer = true)
        {
            var service = messageSender.SendRequest<BufferService>();
            service.WriteToBuffer(pipeType, message, flushBuffer);
        }

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>        
        /// <param name="flushBuffer"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToBufferAsync<T>(this IMessageSender messageSender, PipeType pipeType, T message, bool flushBuffer = true, CancellationToken _ = default)
        {
            messageSender.SendToBuffer(pipeType, message, flushBuffer);
            return Task.CompletedTask;
        }

        public static void SendFlushBufferCommand(this IMessageSender messageSender, PipeType pipeType)
        {
            var service = messageSender.SendRequest<BufferService>();
            service.FlushBuffer(pipeType);
        }

        public static Task SendFlushBufferCommandAsync(this IMessageSender messageSender, PipeType pipeType, CancellationToken _ = default)
        {
            messageSender.SendFlushBufferCommand(pipeType);
            return Task.CompletedTask;
        }

        #endregion

        public static TResponse SendRequest<TResponse>(this IMessageSender messageSender)
        {
            var message = new RequestResponseMessage<TResponse>();
            messageSender.Send(message);
            return message.Response;
        }

        public static async Task<TResponse> SendRequestAsync<TResponse>(this IMessageSender messageSender, CancellationToken cancellationToken = default)
        {
            var message = new RequestResponseMessage<TResponse>();
            await messageSender.SendAsync(message, cancellationToken);
            return message.Response;
        }

        public static TResponse SendRequest<TRequest, TResponse>(this IMessageSender messageSender, TRequest request)
        {
            var message = new RequestResponseMessage<TRequest, TResponse>(request);
            messageSender.Send(message);
            return message.Response;
        }

        public static async Task<TResponse> SendRequestAsync<TRequest, TResponse>(this IMessageSender messageSender, TRequest request, CancellationToken cancellationToken = default)
        {
            var message = new RequestResponseMessage<TRequest, TResponse>(request);
            await messageSender.SendAsync(message, cancellationToken);
            return message.Response;
        }

        #region Pipe extensions

        public static void SendToTransmitPipe<T>(this IMessageSender messageSender, T message) => messageSender.SendToTransmitPipe(null, message);

        public static void SendToTransmitPipe<T>(this IMessageSender messageSender, object portId, T message) => messageSender.SendToPipe(PipeType.Transmit, portId, message);

        public static Task SendToTransmitPipeAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendToTransmitPipeAsync(null, message, cancellationToken);

        public static Task SendToTransmitPipeAsync<T>(this IMessageSender messageSender, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(PipeType.Transmit, portId, message, cancellationToken);

        public static void SendToReceivePipe<T>(this IMessageSender messageSender, T message) => messageSender.SendToReceivePipe(null, message);

        public static void SendToReceivePipe<T>(this IMessageSender messageSender, object portId, T message) => messageSender.SendToPipe(PipeType.Receive, portId, message);

        public static Task SendToReceivePipeAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToReceivePipeAsync(null, message, cancellationToken);

        public static Task SendToReceivePipeAsync<T>(this IMessageSender messageSender, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(PipeType.Receive, portId, message, cancellationToken);

        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, T message) =>
            messageSender.SendToPipe(pipeId, null, message);

        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(pipeId, null, message, cancellationToken);

        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, object portId, T message) => 
            messageSender.Send(new PipeMessage<T>(pipeId, portId, message, messageSender));

        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new PipeMessage<T>(pipeId, portId, message, messageSender), cancellationToken);

        #endregion
    }
}