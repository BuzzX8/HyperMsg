using HyperMsg.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Sends event for received message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Received message.</param>
        public static void SendReceiveEvent<T>(this IMessageSender messageSender, T message) => messageSender.Send(new ReceiveEvent<T>(message));

        /// <summary>
        /// Sends event for received message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Received message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendReceiveEventAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => messageSender.SendAsync(new ReceiveEvent<T>(message), cancellationToken);

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
        public static void SendSerializeCommand<T>(this IMessageSender messageSender, IBufferWriter<byte> bufferWriter, T message) =>
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
        public static Task SendSerializeCommandAsync<T>(this IMessageSender messageSender, IBufferWriter<byte> bufferWriter, T message, CancellationToken cancellationToken = default) =>
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
        public static void SendToBuffer<T>(this IMessageSender messageSender, BufferType bufferType, T message, bool flushBuffer = true) => 
            messageSender.Send(new SendToBufferCommand(handler => handler.WriteToBuffer(bufferType, message, flushBuffer)));

        /// <summary>
        /// Sends message to transmit buffer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>        
        /// <param name="flushBuffer"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendToBufferAsync<T>(this IMessageSender messageSender, BufferType bufferType, T message, bool flushBuffer = true, CancellationToken cancellationToken = default) => 
            messageSender.SendAsync(new SendToBufferCommand(handler => handler.WriteToBuffer(bufferType, message, flushBuffer)), cancellationToken);

        public static void SendFlushBufferCommand(this IMessageSender messageSender, BufferType bufferType) => messageSender.Send(new FlushBufferCommand(bufferType));

        public static Task SendFlushBufferCommandAsync(this IMessageSender messageSender, BufferType bufferType, CancellationToken cancellationToken = default) => 
            messageSender.SendAsync(new FlushBufferCommand(bufferType), cancellationToken);

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

        public static async Task<T> SendWaitForMessageRequest<T>(this IMessagingContext context, Func<T, bool> messagePredicate, CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<T>();
            using var _ = cancellationToken.Register(() => completionSource.SetCanceled());
            using var __ = context.HandlersRegistry.RegisterHandler<T>((message, token) =>
            {
                return Task.Run(() => Task.FromResult(messagePredicate.Invoke(message)))
                    .ContinueWith(completed =>
                    {
                        if (completed.IsCompletedSuccessfully && completed.Result)
                        {
                            completionSource.SetResult(message);
                        }

                        if (completed.IsFaulted)
                        {
                            completionSource.SetException(completed.Exception);
                        }
                    });
            });

            return await completionSource.Task;
        }

        public static IServiceScope SendCreateServiceScopeRequest(this IMessageSender messageSender, Action<IServiceCollection> serviceConfigurator)
        {
            var command = new StartServiceScopeRequest { ServiceConfigurator = serviceConfigurator };
            messageSender.Send(command);
            
            return command.ServiceScope;
        }

        public static async Task<IServiceScope> SendCreateServiceScopeRequestAsync(this IMessageSender messageSender, Action<IServiceCollection> serviceConfigurator, CancellationToken cancellationToken = default)
        {
            var command = new StartServiceScopeRequest { ServiceConfigurator = serviceConfigurator };
            await messageSender.SendAsync(command, cancellationToken);

            return command.ServiceScope;
        }
    }
}