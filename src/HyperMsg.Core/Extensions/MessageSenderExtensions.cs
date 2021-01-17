using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Wraps message into Received<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void Received<T>(this IMessageSender messageSender, T message) => messageSender.Send(new Received(message));

        /// <summary>
        /// Wraps message into Received<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task ReceivedAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Received(message), cancellationToken);

        /// <summary>
        /// Wraps message into Transmit<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void Transmit<T>(this IMessageSender messageSender, T message) => messageSender.Send(new Transmit(message));

        /// <summary>
        /// Wraps message into Transmit<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task TransmitAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Transmit(message), cancellationToken);
    }
}
