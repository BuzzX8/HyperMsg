﻿using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        void Handle<T>(T command);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task HandleAsync<T>(T command, CancellationToken token = default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandler<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        void Handle(T command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task HandleAsync(T command, CancellationToken token = default);
    }    
}