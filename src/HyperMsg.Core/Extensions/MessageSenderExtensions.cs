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
        public static void Receive<T>(this IMessageSender messageSender, T message) => messageSender.Send(new Receive<T>(message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task ReceiveAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Receive<T>(message), cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void Transmit<T>(this IMessageSender messageSender, T message) => messageSender.Send(new Transmit<T>(message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task TransmitAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Transmit<T>(message), cancellationToken);

        public static async Task TransmitAsync(this IMessageSender messageSender, IBuffer transmittingBuffer, CancellationToken cancellationToken)
        {
            var reader = transmittingBuffer.Reader;
            var buffer = reader.Read();

            if (buffer.Length == 0)
            {
                return;
            }

            if (buffer.IsSingleSegment)
            {
                await messageSender.TransmitAsync(buffer.First, cancellationToken);
                reader.Advance((int)buffer.Length);
                return;
            }

            var enumerator = buffer.GetEnumerator();

            while (enumerator.MoveNext())
            {
                await messageSender.TransmitAsync(enumerator.Current, cancellationToken);
            }
        }

        public static IServiceScope CreateServiceScope(this IMessageSender messageSender, Action<IServiceCollection> serviceConfigurator)
        {
            var command = new StartServiceScope { ServiceConfigurator = serviceConfigurator };
            messageSender.Send(command);
            
            return command.ServiceScope;
        }

        public static async Task<IServiceScope> CreateServiceScopeAsync(this IMessageSender messageSender, Action<IServiceCollection> serviceConfigurator, CancellationToken cancellationToken = default)
        {
            var command = new StartServiceScope { ServiceConfigurator = serviceConfigurator };
            await messageSender.SendAsync(command, cancellationToken);

            return command.ServiceScope;
        }
    }
}
