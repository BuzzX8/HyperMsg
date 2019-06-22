using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBufferReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        void Advance(int count);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ReadOnlySequence<byte> Read();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken cancellationToken);
    }
}
