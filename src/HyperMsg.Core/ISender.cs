using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISender<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Send(T message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SendAsync(T message, CancellationToken token);
    }
}
