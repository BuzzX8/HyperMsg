using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReceiver<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T Receive();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<T> ReceiveAsync(CancellationToken token);
    }
}
