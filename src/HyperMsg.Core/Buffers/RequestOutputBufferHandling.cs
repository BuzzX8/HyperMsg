namespace HyperMsg.Buffers;

/// <summary>
/// Represents a request to handle an output buffer operation.
/// Encapsulates the <see cref="IBuffer"/> instance used for writing outgoing data.
/// </summary>
/// <param name="Buffer">The buffer used for output operations.</param>
internal record struct RequestOutputBufferHandling(IBuffer Buffer);