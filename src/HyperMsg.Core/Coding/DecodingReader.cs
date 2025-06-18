namespace HyperMsg.Coding;

using BufferProvider = Func<ReadOnlyMemory<byte>>;

public readonly record struct DecodingResult<T>(T Message, int BytesDecoded);

public delegate DecodingResult<T> Decoder<T>(ReadOnlyMemory<byte> buffer);

public static class DecodingReader
{
    public static Func<DecodingResult<T>> New<T>(Decoder<T> decoder, Memory<byte> buffer) => New(decoder, () => buffer);

    public static Func<DecodingResult<T>> New<T>(Decoder<T> decoder, BufferProvider bufferProvider) => () => decoder.Invoke(bufferProvider());
}
