using System.Threading;
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
        /// <param name="message"></param>
        void Handle<T>(T message);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task HandleAsync<T>(T message, CancellationToken token = default);
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
        /// <param name="message"></param>
        void Handle(T message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task HandleAsync(T message, CancellationToken token = default);
    }    
}