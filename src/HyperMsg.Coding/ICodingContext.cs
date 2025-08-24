namespace HyperMsg.Coding;

/// <summary>
/// Defines a context for encoding and decoding operations for a specific type.
/// </summary>
/// <typeparam name="T">The type of object to encode and decode.</typeparam>
public interface ICodingContext<T>
{
    /// <summary>
    /// Gets the encoder responsible for serializing objects of type <typeparamref name="T"/>.
    /// </summary>
    Encoder<T> Encoder { get; }

    /// <summary>
    /// Gets the decoder responsible for deserializing objects of type <typeparamref name="T"/>.
    /// </summary>
    Decoder<T> Decoder { get; }
}
