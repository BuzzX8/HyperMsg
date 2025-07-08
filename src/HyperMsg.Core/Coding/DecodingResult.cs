namespace HyperMsg.Coding;

public readonly record struct DecodingResult<T>(T Message, ulong BytesDecoded);