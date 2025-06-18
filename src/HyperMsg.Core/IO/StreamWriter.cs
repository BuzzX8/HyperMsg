using HyperMsg.Buffers;

namespace HyperMsg.IO;

public static class StreamWriter
{
    public static MemoryReader New(Stream stream) => New(() => stream);

    public static MemoryReader New(Func<Stream> streamProvider)
    {
        return buffer =>
        {
            var stream = streamProvider();
            stream.Write(buffer.Span);
            return buffer.Length;
        };
    }
}