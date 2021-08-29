using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for registering data handlers.
    /// </summary>
    public interface IHandlersRegistry
    {
        /// <summary>
        /// Registers synchronous data handler.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="dataHandler">Data handler.</param>
        /// <returns>Registration handle. Disposing handle cancels registration.</returns>
        IDisposable RegisterHandler<T>(Action<T> dataHandler);

        /// <summary>
        /// Registers asynchronous data handler.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="dataHandler">Data handler.</param>
        /// <returns>Registration handle. Disposing handle cancels registration.</returns>
        IDisposable RegisterHandler<T>(AsyncAction<T> dataHandler);
    }
}