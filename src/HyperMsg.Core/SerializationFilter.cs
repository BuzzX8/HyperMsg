namespace HyperMsg;

public class SerializationFilter
{
    private readonly Dictionary<Type, object> serializers = new();

    public void Register<T>(Action<IBufferWriter, T> serializer)
    {
        Deregister<T>();

        serializers[typeof(T)] = serializer;
    }

    public void Deregister<T>()
    {
        if (!serializers.ContainsKey(typeof(T)))
            return;
        
        serializers.Remove(typeof(T));
    }

    public void Serialize<T>(IBufferWriter writer, T message)
    {
        if (!serializers.ContainsKey(typeof(T)))
        {
            return;
        }

        var serializer = (Action<IBufferWriter, T>)serializers[typeof(T)];

        serializer.Invoke(writer, message);
    }
}
