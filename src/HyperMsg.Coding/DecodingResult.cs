namespace HyperMsg.Coding;

/// <summary>
/// Represents the result of a decoding operation, including the decoded message and the number of bytes processed.
/// </summary>
/// <typeparam name="T">The type of the decoded message.</typeparam>
/// <param name="Message">The decoded message instance.</param>
/// <param name="BytesDecoded">The number of bytes that were successfully decoded.</param>
public readonly record struct DecodingResult<T>(T Message, ulong BytesDecoded);