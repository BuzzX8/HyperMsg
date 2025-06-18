namespace HyperMsg.Coding;

using BufferProvider = Func<Memory<byte>>;

public delegate long Encoder<T>(Memory<byte> buffer, T message);

public static class EncodingWriter
{
    public static Func<T, long> New<T>(Encoder<T> encoder, Memory<byte> buffer)
        => New(encoder, () => buffer);

    public static Func<T, long> New<T>(Encoder<T> encoder, BufferProvider bufferProvider)
    {
        return message => encoder.Invoke(bufferProvider(), message);
    }
}
