namespace HyperMsg.Coding;

public record CodingContext<T>(Encoder<T> encoder, Decoder<T> decoder)
{
    public Encoder<T> Encoder { get; private set; } = encoder;
    public Decoder<T> Decoder { get; private set; } = decoder;
}
