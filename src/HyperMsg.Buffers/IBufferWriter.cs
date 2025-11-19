using System.Buffers;

namespace HyperMsg.Buffers;

/// <summary>
/// Defines methods for writing data to a buffer.
/// Inherits from <see cref="IBufferWriter{T}"/> for <see cref="byte"/> data.
/// </summary>
public interface IBufferWriter : IBufferWriter<byte>
{ }