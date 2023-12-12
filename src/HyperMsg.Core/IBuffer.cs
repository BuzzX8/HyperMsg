namespace HyperMsg;

/// <summary>
/// Defines methods for basic operations with byte buffer and
/// receiving notifications about flush request.
/// </summary>
public interface IBuffer
{
    /// <summary>
    /// Returns reader for buffer.
    /// </summary>
    IBufferReader Reader { get; }

    /// <summary>
    /// Returns writer for buffer.
    /// </summary>
    IBufferWriter Writer { get; }

    /// <summary>
    /// Clears buffer data.
    /// </summary>
    void Clear();
}