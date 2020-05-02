using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Wraps message into Received<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void Received<T>(this ISender messageSender, T message) => messageSender.Send(new Received<T>(message));

        /// <summary>
        /// Wraps message into Received<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task ReceivedAsync<T>(this ISender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Received<T>(message), cancellationToken);

        public static void BufferReceivedData(this ISender messageSender, IBuffer receivingBuffer) => messageSender.Received(receivingBuffer);

        public static Task BufferReceivedDataAsync(this ISender messageSender, IBuffer receivingBuffer, CancellationToken cancellationToken) => messageSender.ReceivedAsync(receivingBuffer, cancellationToken);

        /// <summary>
        /// Wraps message into Transmit<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void Transmit<T>(this ISender messageSender, T message) => messageSender.Send(new Transmit<T>(message));

        /// <summary>
        /// Wraps message into Transmit<typeparamref name="T"/> decorator and sends.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task TransmitAsync<T>(this ISender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Transmit<T>(message), cancellationToken);

        public static void TransmitBufferData(this ISender messageSender, IBuffer transmittingBuffer) => messageSender.Transmit(transmittingBuffer);

        public static Task TransmitBufferDataAsync(this ISender messageSender, IBuffer transmittingBuffer, CancellationToken cancellationToken) => messageSender.TransmitAsync(transmittingBuffer, cancellationToken);
    }
}
