﻿using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessageBuffer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Write(T message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FlushAsync(CancellationToken cancellationToken);
    }
}