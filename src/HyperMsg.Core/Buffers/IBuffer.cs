namespace HyperMsg.Buffers;

/// <summary>
/// Represents a buffer that provides both reading and writing capabilities, with reactive behavior.
/// </summary>
public interface IBuffer : IBufferReader, IBufferWriter
{
    IBufferReader Reader { get; }
    IBufferWriter Writer { get; }
    void Clear();

    /// <summary>
    /// Occurs when data is written to the buffer.
    /// </summary>
    event Action<int> DataAppended;

    /// <summary>
    /// Occurs when data is read from the buffer.
    /// </summary>
    event Action<int> DataRead;
}
