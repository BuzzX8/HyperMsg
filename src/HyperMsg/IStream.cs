﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStream
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int Read(Memory<byte> buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        void Write(Memory<byte> buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    }
}
