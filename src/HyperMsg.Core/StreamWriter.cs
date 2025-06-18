namespace HyperMsg;

public static class StreamWriter
{
    public static Action<ReadOnlyMemory<byte>> New(Stream stream) => New(() => stream);

    public static Action<ReadOnlyMemory<byte>> New(Func<Stream> streamProvider)
    {
        return buffer =>
        {
            var stream = streamProvider();
            stream.Write(buffer.Span);
        };
    }
}
