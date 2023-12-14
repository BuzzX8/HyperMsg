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

    Memory<byte> GetMemory();

    Span<byte> GetSpan();
}