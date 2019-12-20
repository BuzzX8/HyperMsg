using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for registering message handlers.
    /// </summary>
    public interface IMessageHandlerRegistry
    {
        /// <summary>
        /// Registers synchronous message handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="handler">Message handler.</param>
        void Register<T>(Action<T> handler);

        /// <summary>
        /// Registers asynchronous message handler.
        /// </summary>
        /// <typeparam name="T">Typeof message.</typeparam>
        /// <param name="handler">Message handler.</param>
        void Register<T>(AsyncAction<T> handler);
    }
}