namespace HyperMsg;

public static class StreamReader
{
    public static Func<Memory<byte>, Result<Memory<byte>>> New(Stream stream) => New(() => stream);

    public static Func<Memory<byte>, Result<Memory<byte>>> New(Func<Stream> streamProvider)
    {
        return buffer =>
        {
            var stream = streamProvider();
            var bytesRead = stream.Read(buffer.Span);
            return new Result<Memory<byte>>(buffer[..bytesRead]);
        };
    }

    public static Func<Result<Memory<byte>>> New(Func<Stream> streamProvider, Func<Result<Memory<byte>>> bufferProvider)
    {
        var streamReader = New(streamProvider);

        return () => bufferProvider().Match(
            Succ: buffer => streamReader(buffer),
            Fail: error => new Result<Memory<byte>>(error));
    }

    public static Func<Memory<byte>, CancellationToken, ValueTask<Result<Memory<byte>>>> NewAsync(Stream stream) => NewAsync(() => stream);

    public static Func<Memory<byte>, CancellationToken, ValueTask<Result<Memory<byte>>>> NewAsync(Func<Stream> streamProvider)
    {
        return async (buffer, token) =>
        {
            var stream = streamProvider();
            var bytesRead = await stream.ReadAsync(buffer, token);
            return new Result<Memory<byte>>(buffer[..bytesRead]);
        };
    }

    public static Func<CancellationToken, ValueTask<Result<Memory<byte>>>> NewAsync(Func<Stream> streamProvider, Func<Result<Memory<byte>>> bufferProvider)
    {
        var streamReader = NewAsync(streamProvider);

        return async token => await bufferProvider().Match(
            Succ: buffer => streamReader(buffer, token),
            Fail: error => ValueTask.FromResult(new Result<Memory<byte>>(error)));
    }
}