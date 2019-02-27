using System;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageObserver<T>
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler MessageReceived;
    }
}
