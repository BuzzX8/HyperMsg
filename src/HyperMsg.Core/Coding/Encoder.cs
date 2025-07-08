namespace HyperMsg.Coding;

public delegate ulong Encoder<T>(Memory<byte> buffer, T message);