using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for registering message observers.
    /// </summary>
    public interface IMessageObservable
    {
        /// <summary>
        /// Registers synchronous message handler.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageObserver">Message observer.</param>
        /// <returns></returns>
        IDisposable AddObserver<T>(Action<T> messageObserver);

        /// <summary>
        /// Registers asynchronous message handler.
        /// </summary>
        /// <typeparam name="T">Typeof message.</typeparam>
        /// <param name="messageObserver">Message observer.</param>
        /// <returns></returns>
        IDisposable AddObserver<T>(AsyncAction<T> messageObserver);
    }
}