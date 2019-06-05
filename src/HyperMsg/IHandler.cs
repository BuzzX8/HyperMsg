using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
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
        Task HandleAsync(T message, CancellationToken cancellationToken);
    }    
}