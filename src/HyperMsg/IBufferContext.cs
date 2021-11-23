namespace HyperMsg;

/// <summary>
/// Represents buffers designated for transmitting and receiving data.
/// </summary>
public interface IBufferContext
{
    /// <summary>
    /// Buffer for receiving incoming data.
    /// </summary>
    IBuffer ReceivingBuffer { get; }

    /// <summary>
    /// Buffer for transmitting data.
    /// </summary>
    IBuffer TransmittingBuffer { get; }
}