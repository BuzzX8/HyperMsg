namespace HyperMsg;

public delegate Result<(T message, int bytesDecoded)> Decoder<T>(ReadOnlyMemory<byte> buffer);
