namespace HyperMsg.Coding;

internal class CodingContext<T>(Encoder<T> encoder, Decoder<T> decoder) : ICodingContext<T>
{
    public Encoder<T> Encoder { get; } = encoder;
    public Decoder<T> Decoder { get; } = decoder;
}
