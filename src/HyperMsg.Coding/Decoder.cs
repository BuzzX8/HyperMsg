namespace HyperMsg.Coding;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="buffer"></param>
/// <returns></returns>
public delegate DecodingResult<T> Decoder<T>(ReadOnlyMemory<byte> buffer);