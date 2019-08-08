using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBufferReader<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        void Advance(int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ReadOnlySequence<T>> ReadAsync(CancellationToken cancellationToken);
    }
}
