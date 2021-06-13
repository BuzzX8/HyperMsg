using HyperMsg.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to transmit.</param>
        public static void SendToTransmitBuffer<T>(this IMessageSender messageSender, T message) => messageSender.SendToBuffer(PipeType.Transmit, message);

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(PipeType.Transmit, message, true, cancellationToken);

        public static void SendToReceiveBuffer<T>(this IMessageSender messageSender, T message) => messageSender.SendToBuffer(PipeType.Receive, message);

        public static Task SendToReceiveBuffer<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(PipeType.Receive, message);

        /// <summary>
        /// Sends command for message serialization.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="bufferWriter">Writer for serialization.</param>
        /// <param name="message">Message to serialize.</param>
        public static void SendSerializeCommand<T>(this IMessageSender messageSender, IBufferWriter bufferWriter, T message) =>
            messageSender.Send(new SerializeCommand<T>(bufferWriter, message));

        /// <summary>
        /// Sends command for message serialization.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Writer for serialization.</param>
        /// <param name="bufferWriter">Writer for serialization.</param>
        /// <param name="message">Message to serialize.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendSerializeCommandAsync<T>(this IMessageSender messageSender, IBufferWriter bufferWriter, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new SerializeCommand<T>(bufferWriter, message), cancellationToken);

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

        public static void SendToTransmitPipe<T>(this IMessageSender messageSender, T message) => messageSender.SendToPipe(PipeType.Transmit, message);

        public static Task SendToTransmitPipeAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendToPipeAsync(PipeType.Transmit, message, cancellationToken);

        public static void SendToReceivePipe<T>(this IMessageSender messageSender, T message) => messageSender.SendToPipe(PipeType.Receive, message);

        public static Task SendToReceivePipeAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(PipeType.Receive, message, cancellationToken);

        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, T message) =>
            messageSender.SendToPipe(pipeId, null, message);

        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendToPipeAsync(pipeId, null, message, cancellationToken);

        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, object portId, T message) => 
            messageSender.Send(new PipeMessage<T>(pipeId, portId, message));

        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new PipeMessage<T>(pipeId, portId, message), cancellationToken);
    }
}