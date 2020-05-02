using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for sending messages.
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Sends message.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="message">Message to send.</param>
        void Send<T>(T message);

        /// <summary>
        /// Sends message asynchronous.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that represents async status of send operation.</returns>
        Task SendAsync<T>(T message, CancellationToken cancellationToken);
    }
}
