namespace HyperMsg;

using BufferProvider = Func<Fin<Memory<byte>>>;
using static Fin<ReadOnlyMemory<byte>>;
using LanguageExt;

public delegate Fin<int> Encoder<T>(Memory<byte> buffer, T message);

public static class EncodingWriter
{
    public static Func<T, Fin<ReadOnlyMemory<byte>>> New<T>(Encoder<T> encoder, Memory<byte> buffer)
        => New(encoder, () => Fin<Memory<byte>>.Succ(buffer));

    public static Func<T, Fin<ReadOnlyMemory<byte>>> New<T>(Encoder<T> encoder, BufferProvider bufferProvider)
    {
        return message => bufferProvider().Match(
            Succ: buffer => encoder(buffer, message).Match(
                Succ: bytesEncoded => Succ(buffer[..bytesEncoded]),
                Fail: error => Fail(error)),
            Fail: error => Fail(error));
    }
}
