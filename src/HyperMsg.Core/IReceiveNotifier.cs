using System;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReceiveNotifier<T>
    {
        /// <summary>
        /// 
        /// </summary>
        event Action<T> Received;
    }
}
