namespace HyperMsg;

public static class StreamWriter
{
    public static Func<ReadOnlyMemory<byte>, Result<Unit>> New(Stream stream) => New(() => stream);

    public static Func<ReadOnlyMemory<byte>, Result<Unit>> New(Func<Stream> streamProvider)
    {
        return buffer =>
        {
            var stream = streamProvider();
            stream.Write(buffer.Span);
            return new Result<Unit>(Unit.Default);
        };
    }

    public static Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask<Result<Unit>>> NewAsync(Stream stream) => NewAsync(() => stream);

    public static Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask<Result<Unit>>> NewAsync(Func<Stream> streamProvider)
    {
        return async (buffer, token) =>
        {
            var stream = streamProvider();
            await stream.WriteAsync(buffer, token);
            return new Result<Unit>(Unit.Default);
        };
    }
}
