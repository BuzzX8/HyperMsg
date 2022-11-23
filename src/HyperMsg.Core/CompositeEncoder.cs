namespace HyperMsg;

public class CompositeEncoder : IEncoder
{
    private readonly Dictionary<Type, object> encoders = new();

    public void Add<T>(Encoder<T> encoder)
    {
        Remove<T>();

        encoders[typeof(T)] = encoder;
    }

    public void Remove<T>()
    {
        if (!encoders.ContainsKey(typeof(T)))
            return;
        
        encoders.Remove(typeof(T));
    }

    public void Encode<T>(IBufferWriter writer, T message)
    {
        if (!encoders.ContainsKey(typeof(T)))
        {
            return;
        }

        var serializer = (Encoder<T>)encoders[typeof(T)];

        serializer.Invoke(writer, message);
    }
}
