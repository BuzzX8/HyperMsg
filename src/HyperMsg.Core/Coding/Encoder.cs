namespace HyperMsg.Coding;

/// <summary>
/// Represents a method that encodes a message of type <typeparamref name="T"/> into a byte buffer.
/// </summary>
/// <typeparam name="T">The type of the message to encode.</typeparam>
/// <param name="buffer">The buffer to write the encoded message into.</param>
/// <param name="message">The message instance to encode.</param>
/// <returns>The number of bytes written to the buffer.</returns>
public delegate ulong Encoder<T>(Memory<byte> buffer, T message);