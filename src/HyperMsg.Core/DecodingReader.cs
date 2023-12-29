namespace HyperMsg;

using BufferProvider = Func<Fin<ReadOnlyMemory<byte>>>;
using static Fin<ReadOnlyMemory<byte>>;

public readonly record struct DecodingResult<T>(T Message, int BytesDecoded);

public delegate Fin<DecodingResult<T>> Decoder<T>(ReadOnlyMemory<byte> buffer);

public static class DecodingReader
{
    public static Func<Fin<DecodingResult<T>>> New<T>(Decoder<T> decoder, Memory<byte> buffer) => New(decoder, () => Succ(buffer));

    public static Func<Fin<DecodingResult<T>>> New<T>(Decoder<T> decoder, BufferProvider bufferProvider)
    {
        return  () => bufferProvider().Match(
            Succ: buffer => decoder(buffer),
            Fail: error => Fin<DecodingResult<T>>.Fail(error));
    }
}
