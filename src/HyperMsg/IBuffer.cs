using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for basic operations with byte buffer and
    /// receiving notifications about flush request.
    /// </summary>
    public interface IBuffer
    {
        /// <summary>
        /// Returns reader for buffer.
        /// </summary>
        IBufferReader<byte> Reader { get; }

        /// <summary>
        /// Returns writer for buffer.
        /// </summary>
        IBufferWriter<byte> Writer { get; }

        /// <summary>
        /// Clears buffer data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Initiates buffer flush operation.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which completes when flush operation finished.</returns>
        Task FlushAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Rises when buffer flush requested.
        /// </summary>
        event AsyncAction<IBufferReader<byte>> FlushRequested;
    }
}