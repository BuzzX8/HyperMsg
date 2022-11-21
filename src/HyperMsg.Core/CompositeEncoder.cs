namespace HyperMsg;

public class CompositeEncoder : IEncoder
{
    private readonly Dictionary<Type, object> encoders = new();

    public void Register<T>(Action<IBufferWriter, T> serializer)
    {
        Deregister<T>();

        encoders[typeof(T)] = serializer;
    }

    public void Deregister<T>()
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

        var serializer = (Action<IBufferWriter, T>)encoders[typeof(T)];

        serializer.Invoke(writer, message);
    }
}
