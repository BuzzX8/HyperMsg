namespace HyperMsg.Coding;

public class CodingContext<T>
{
    internal CodingContext(Encoder<T> encoder, Decoder<T> decoder)
    {
        Encoder = encoder;
        Decoder = decoder;
    }

    public Encoder<T> Encoder { get; private set; }
    public Decoder<T> Decoder { get; private set; }
}
