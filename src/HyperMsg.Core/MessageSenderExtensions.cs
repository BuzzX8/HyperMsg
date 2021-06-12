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
        public static void SendToTransmitBuffer<T>(this IMessageSender messageSender, T message) => messageSender.SendToBuffer(BufferType.Transmitting, message);

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToTransmitBufferAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendToBufferAsync(BufferType.Transmitting, message, true, cancellationToken);

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
        /// <param name="bufferType">Buffer type.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="flushBuffer"></param>
        /// <returns></returns>
        public static void SendToBuffer<T>(this IMessageSender messageSender, BufferType bufferType, T message, bool flushBuffer = true)
        {
            var service = messageSender.SendRequest<BufferService>();
            service.WriteToBuffer(bufferType, message, flushBuffer);
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
        public static Task SendToBufferAsync<T>(this IMessageSender messageSender, BufferType bufferType, T message, bool flushBuffer = true, CancellationToken cancellationToken = default)
        {
            messageSender.SendToBuffer(bufferType, message, flushBuffer);
            return Task.CompletedTask;
        }

        public static void SendFlushBufferCommand(this IMessageSender messageSender, BufferType bufferType)
        {
            var service = messageSender.SendRequest<BufferService>();
            service.FlushBuffer(bufferType);
        }

        public static Task SendFlushBufferCommandAsync(this IMessageSender messageSender, BufferType bufferType, CancellationToken cancellationToken = default)
        {
            messageSender.SendFlushBufferCommand(bufferType);
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

        public static void SendToPipe<T>(this IMessageSender messageSender, object pipeId, object portId, T message) => 
            messageSender.Send(new PipeMessage<T>(pipeId, portId, message));

        public static Task SendToPipeAsync<T>(this IMessageSender messageSender, object pipeId, object portId, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new PipeMessage<T>(pipeId, portId, message), cancellationToken);
    }
}