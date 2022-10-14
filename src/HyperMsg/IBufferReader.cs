namespace HyperMsg;

/// <summary>
/// Defines methods for reading data from buffer.
/// </summary>
public interface IBufferReader
{
    /// <summary>
    /// Advances reading position on specified number of elements.
    /// </summary>
    /// <param name="count">Number of elements.</param>
    void Advance(int count);

    /// <summary>
    /// Returns list of buffer chunks wich contains data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of buffer chunks with data.</returns>
    ReadOnlyMemory<byte> Read();
}
