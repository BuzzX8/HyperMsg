using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for registering message handlers.
    /// </summary>
    public interface IMessageHandlersRegistry
    {
        /// <summary>
        /// Registers synchronous message handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageHandler">Message handler.</param>
        /// <returns>Registration handle. Disposing handle cancels registration.</returns>
        IDisposable RegisterHandler<T>(Action<T> messageHandler);

        /// <summary>
        /// Registers asynchronous message handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageHandler">Message handler.</param>
        /// <returns>Registration handle. Disposing handle cancels registration.</returns>
        IDisposable RegisterHandler<T>(AsyncAction<T> messageHandler);
    }
}