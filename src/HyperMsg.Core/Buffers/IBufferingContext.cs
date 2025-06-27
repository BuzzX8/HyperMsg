namespace HyperMsg.Buffers;

/// <summary>
/// Provides context for buffer management, exposing input and output buffers.
/// </summary>
public interface IBufferingContext
{
    /// <summary>
    /// Gets the input buffer used for reading data.
    /// </summary>
    IBuffer Input { get; }
    
    /// <summary>
    /// Gets the output buffer used for writing data.
    /// </summary>
    IBuffer Output { get; }
}
