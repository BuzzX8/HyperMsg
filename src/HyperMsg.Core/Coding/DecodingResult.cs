namespace HyperMsg.Coding;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Message"></param>
/// <param name="BytesDecoded"></param>
public readonly record struct DecodingResult<T>(T Message, int BytesDecoded);