using System.Threading;
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
        void Flush();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task FlushAsync(CancellationToken token = default);
    }
}