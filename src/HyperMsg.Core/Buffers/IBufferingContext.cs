namespace HyperMsg.Buffers;

/// <summary>
/// Provides context for buffer management, exposing reader and writer interfaces.
/// </summary>
public interface IBufferingContext
{
    IBuffer Input { get; }
    
    IBuffer Output { get; }
}
