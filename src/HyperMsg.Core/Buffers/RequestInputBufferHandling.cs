namespace HyperMsg.Buffers;

/// <summary>
/// Represents a request to handle an input buffer operation.
/// Encapsulates the <see cref="IBuffer"/> instance used for reading incoming data.
/// </summary>
/// <param name="Buffer">The buffer used for input operations.</param>
public record struct RequestInputBufferHandling(IBuffer Buffer);