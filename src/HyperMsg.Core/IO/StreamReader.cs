using HyperMsg.Buffers;

namespace HyperMsg.IO;

public static class StreamReader
{
    public static MemoryWriter New(Stream stream) => New(() => stream);

    public static MemoryWriter New(Func<Stream> streamProvider)
    {
        return buffer =>
        {
            var stream = streamProvider();
            var bytesRead = stream.Read(buffer.Span);
            return bytesRead;
        };
    }
}