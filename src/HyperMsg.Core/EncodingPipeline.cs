namespace HyperMsg;

public static class EncodingPipeline
{
    public static Func<T, Result<Unit>> New<T>(Encoder<T> encoder, Func<Memory<byte>, Result<Unit>> bufferWriter, Memory<byte> buffer)
        => New(encoder, bufferWriter, () => new Result<Memory<byte>>(buffer));

    public static Func<T, Result<Unit>> New<T>(Encoder<T> encoder, Func<Memory<byte>, Result<Unit>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return message => bufferProvider().Match(
            Succ: buffer => encoder(buffer, message).Match(
                Succ: bytesEncoded => bufferWriter(buffer[..bytesEncoded]),
                Fail: error => new Result<Unit>(error)),
            Fail: error => new Result<Unit>(error));
    }

    public static Func<T, CancellationToken, ValueTask<Result<Unit>>> NewAsync<T>(Encoder<T> encoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<Unit>>> bufferWriter, Memory<byte> buffer)
        => NewAsync(encoder, bufferWriter, () => new Result<Memory<byte>>(buffer));

    public static Func<T, CancellationToken, ValueTask<Result<Unit>>> NewAsync<T>(Encoder<T> encoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<Unit>>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return (message, token) => bufferProvider().Match(
            Succ: buffer => encoder(buffer, message).Match(
                Succ: bytesEncoded => bufferWriter(buffer[..bytesEncoded], token),
                Fail: error => ValueTask.FromResult(new Result<Unit>(error))),
            Fail: error => ValueTask.FromResult(new Result<Unit>(error)));
    }
}
