namespace HyperMsg;

public static class DecodingReader
{
    public static Func<Result<T>> New<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, Func<Memory<byte>, Result<int>> bufferWriter, Memory<byte> buffer)
    {
        return New(decoder, bufferWriter, () => new Result<Memory<byte>>(buffer));
    }

    public static Func<Result<T>> New<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, Func<Memory<byte>, Result<int>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return () => bufferProvider().Match(
            Succ: buffer => bufferWriter(buffer).Match(
                Succ: bytesWritten => decoder(buffer[..bytesWritten]).Match(
                    Succ: r => new Result<T>(r.message),
                    Fail: error => new Result<T>(error)),
                Fail: error => new Result<T>(error)),
            Fail: error => new Result<T>(error));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<int>>> bufferWriter, Memory<byte> buffer)
    {
        return NewAsync(decoder, bufferWriter, () => new Result<Memory<byte>>(buffer));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, Func<Memory<byte>, CancellationToken, ValueTask<Result<int>>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return token => bufferProvider().Match(
            Succ: (Func<Memory<byte>, ValueTask<Result<T>>>)(async (buffer) => await (await bufferWriter(buffer, token)).Match(
                Succ: bytesWritten => decoder(buffer[..bytesWritten]).Match(
                    Succ: r => ValueTask.FromResult(new Result<T>(r.message)),
                    Fail: error => ValueTask.FromResult(new Result<T>(error))),
                Fail: error => ValueTask.FromResult(new Result<T>(error)))),
            Fail: error => ValueTask.FromResult(new Result<T>(error)));
    }
}
