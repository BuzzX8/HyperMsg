namespace HyperMsg;

public static class DecodingPipeline
{
    public static Func<Result<T>> New<T>(Decoder<T> decoder, Func<Memory<byte>, Result<int>> bufferWriter, Memory<byte> buffer)
    {
        return New(decoder, bufferWriter, () => new Result<Memory<byte>>(buffer));
    }

    public static Func<Result<T>> New<T>(Decoder<T> decoder, Func<Memory<byte>, Result<int>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return () => bufferProvider().Match(
            Succ: New(decoder, bufferWriter),
            Fail: error => new Result<T>(error));
    }

    private static Func<Memory<byte>, Result<T>> New<T>(Decoder<T> decoder, Func<Memory<byte>, Result<int>> bufferWriter)
    {
        return buffer => bufferWriter(buffer).Match(
            Succ: New(decoder, buffer),
            Fail: error => new Result<T>(error));
    }

    private static Func<int, Result<T>> New<T>(Decoder<T> decoder, Memory<byte> buffer)
    {
        return bytesWritten => decoder(buffer[..bytesWritten]).Match(
            Succ: r => new Result<T>(r.message),
            Fail: error => new Result<T>(error));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Decoder<T> decoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<int>>> bufferWriter, Memory<byte> buffer)
    {
        return NewAsync(decoder, bufferWriter, () => new Result<Memory<byte>>(buffer));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Decoder<T> decoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<int>>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return token => bufferProvider().Match(
                Succ: NewAsync(decoder, bufferWriter, token),
                Fail: error => ValueTask.FromResult(new Result<T>(error)));
    }

    private static Func<Memory<byte>, ValueTask<Result<T>>> NewAsync<T>(Decoder<T> decoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<int>>> bufferWriter, CancellationToken cancellationToken)
    {
        return async (buffer) => await (await bufferWriter(buffer, cancellationToken)).Match(
                Succ: NewAsync(decoder, buffer),
                Fail: error => ValueTask.FromResult(new Result<T>(error)));
    }

    private static Func<int, ValueTask<Result<T>>> NewAsync<T>(Decoder<T> decoder, Memory<byte> buffer)
    {
        return (bytesWritten) => decoder(buffer[..bytesWritten]).Match(
            Succ: r => ValueTask.FromResult(new Result<T>(r.message)),
            Fail: error => ValueTask.FromResult(new Result<T>(error)));
    }
}
