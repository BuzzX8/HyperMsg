namespace HyperMsg.IO;

public static class StreamReader
{
    public static Func<Memory<byte>, long> New(Stream stream) => New(() => stream);

    public static Func<Memory<byte>, long> New(Func<Stream> streamProvider)
    {
        return buffer =>
        {
            var stream = streamProvider();
            var bytesRead = stream.Read(buffer.Span);
            return bytesRead;
        };
    }
}