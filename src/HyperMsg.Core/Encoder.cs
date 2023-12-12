namespace HyperMsg;

public delegate Result<int> Encoder<T>(Memory<byte> buffer, T message);