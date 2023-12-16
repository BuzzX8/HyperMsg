namespace HyperMsg;

using BufferProvider = Func<Result<Memory<byte>>>;
using ByteWriter = Func<Memory<byte>, Result<Unit>>;
using AsyncByteWriter = Func<Memory<byte>, CancellationToken, ValueTask<Result<Unit>>>;

public static class EncodingWriter
{
    public static Func<T, Result<Unit>> New<T>(Func<Memory<byte>, T, Result<int>> encoder, ByteWriter byteWriter, Memory<byte> buffer)
        => New(encoder, byteWriter, () => new Result<Memory<byte>>(buffer));

    public static Func<T, Result<Unit>> New<T>(Func<Memory<byte>, T, Result<int>> encoder, ByteWriter byteWriter, BufferProvider bufferProvider)
    {
        return message => bufferProvider().Match(
            Succ: buffer => encoder(buffer, message).Match(
                Succ: bytesEncoded => byteWriter(buffer[..bytesEncoded]),
                Fail: error => new(error)),
            Fail: error => new(error));
    }

    public static Func<T, CancellationToken, ValueTask<Result<Unit>>> NewAsync<T>(Func<Memory<byte>, T, Result<int>> encoder, AsyncByteWriter byteWriter, Memory<byte> buffer)
        => NewAsync(encoder, byteWriter, () => new(buffer));

    public static Func<T, CancellationToken, ValueTask<Result<Unit>>> NewAsync<T>(Func<Memory<byte>, T, Result<int>> encoder, AsyncByteWriter byteWriter, BufferProvider bufferProvider)
    {
        return (message, token) => bufferProvider().Match(
            Succ: buffer => encoder(buffer, message).Match(
                Succ: bytesEncoded => byteWriter(buffer[..bytesEncoded], token),
                Fail: error => ValueTask.FromResult(new Result<Unit>(error))),
            Fail: error => ValueTask.FromResult(new Result<Unit>(error)));
    }
}
