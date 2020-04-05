﻿using System;

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
        void Subscribe<T>(Action<T> messageObserver);

        /// <summary>
        /// Registers asynchronous message handler.
        /// </summary>
        /// <typeparam name="T">Typeof message.</typeparam>
        /// <param name="messageObserver">Message observer.</param>
        void Subscribe<T>(AsyncAction<T> messageObserver);
    }
}