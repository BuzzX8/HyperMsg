namespace HyperMsg;

public static class DecodingPipeline
{
    public static Func<Result<T>> New<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, Func<Result<Memory<byte>>> bufferProvider)
    {
        return () => bufferProvider().Match(
            Succ: buffer => decoder(buffer).Match(
                Succ: r => new Result<T>(r.message),
                Fail: error => new Result<T>(error)),
            Fail: error => new Result<T>(error));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, Func<CancellationToken, ValueTask<Result<Memory<byte>>>> bufferProvider)
    {
        return async token => (await bufferProvider(token)).Match(
            Succ: buffer => decoder(buffer).Match(
                Succ: r => new Result<T>(r.message),
                Fail: error => new Result<T>(error)),
            Fail: error => new Result<T>(error));
            
    }
}
