using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendMessageReceivedEvent<T>(this IMessageSender messageSender, T message) => messageSender.Send(new MessageReceivedEvent<T>(message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendMessageReceivedEventAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new MessageReceivedEvent<T>(message), cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendTransmitMessageCommand<T>(this IMessageSender messageSender, T message) => messageSender.Send(new TransmitMessageCommand<T>(message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendTransmitMessageCommandAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new TransmitMessageCommand<T>(message), cancellationToken);

        public static async Task SendTransmitBufferDataCommandAsync(this IMessageSender messageSender, IBuffer transmittingBuffer, CancellationToken cancellationToken)
        {
            var reader = transmittingBuffer.Reader;
            var buffer = reader.Read();

            if (buffer.Length == 0)
            {
                return;
            }

            if (buffer.IsSingleSegment)
            {
                await messageSender.SendTransmitMessageCommandAsync(buffer.First, cancellationToken);
                reader.Advance((int)buffer.Length);
                return;
            }

            var enumerator = buffer.GetEnumerator();

            while (enumerator.MoveNext())
            {
                await messageSender.SendTransmitMessageCommandAsync(enumerator.Current, cancellationToken);
            }
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
