namespace HyperMsg.Buffers;

/// <summary>
/// Represents a buffer that provides both reading and writing capabilities.
/// </summary>
public interface IBuffer
{
    /// <summary>
    /// Gets the <see cref="IBufferReader"/> interface for reading operations.
    /// </summary>
    IBufferReader Reader { get; }

    /// <summary>
    /// Gets the <see cref="IBufferWriter"/> interface for writing operations.
    /// </summary>
    IBufferWriter Writer { get; }

    /// <summary>
    /// Clears the buffer, resetting its state for reuse.
    /// </summary>
    void Clear();
}
