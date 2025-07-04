namespace HyperMsg.Buffers;

/// <summary>
/// Defines methods for reading data from a buffer.
/// </summary>
public interface IBufferReader
{
    /// <summary>
    /// Advances the read position by the specified number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is negative or exceeds the available data.
    /// </exception>
    void Advance(int count);

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> representing the data available for reading.
    /// </summary>
    /// <returns>A memory segment containing the readable data.</returns>
    Memory<byte> GetMemory();

    /// <summary>
    /// Gets a <see cref="Span{T}"/> representing the data available for reading.
    /// </summary>
    /// <returns>A span containing the readable data.</returns>
    Span<byte> GetSpan();
}