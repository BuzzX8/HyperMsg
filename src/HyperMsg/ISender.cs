using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISender<in T>
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync(T message, CancellationToken cancellationToken);
    }
}
