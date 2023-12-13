namespace HyperMsg;

public static class Reader
{
    public static Func<Result<T>> New<T>(Decoder<T> decoder, Func<Memory<byte>, Result<int>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return () => bufferProvider().Match(
            Succ: NewMethod(decoder, bufferWriter),
            Fail: error => new Result<T>(error));
    }

    private static Func<Memory<byte>, Result<T>> NewMethod<T>(Decoder<T> decoder, Func<Memory<byte>, Result<int>> bufferWriter)
    {
        return buffer => bufferWriter(buffer).Match(
            Succ: NewMethod1(decoder, buffer), 
            Fail: error => new Result<T>(error));
    }

    private static Func<int, Result<T>> NewMethod1<T>(Decoder<T> decoder, Memory<byte> buffer)
    {
        return bytesWritten => decoder(buffer[..bytesWritten]).Match(
            Succ: r => new Result<T>(r.message),
            Fail: error => new Result<T>(error));
    }

    public static Func<CancellationToken, Task<Result<T>>> NewAsync<T>(Decoder<T> decoder, Func<Memory<byte>, CancellationToken, Task<Result<int>>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return token => bufferProvider().Match(
            Succ: null,
            Fail: error => Task.FromResult(new Result<T>(error)));
    }
}
