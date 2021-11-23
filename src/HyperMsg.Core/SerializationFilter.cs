namespace HyperMsg;

internal class SerializationFilter : MessageFilter, ISerializationFilter
{
    private readonly Dictionary<Type, Delegate> serializers;
    private readonly BufferType bufferType;
    private readonly IBuffer buffer;

    internal SerializationFilter(IBuffer buffer, BufferType bufferType, ISender sender) : base(sender)
    {
        this.buffer = buffer;
        this.bufferType = bufferType;
        serializers = new();
    }

    public void AddSerializer<T>(Action<IBufferWriter, T> serializer)
        => serializers.Add(typeof(T), serializer);

    public void RemoveSerializer<T>() => serializers.Remove(typeof(T));

    protected override bool HandleMessage<T>(T message)
    {
        if (!serializers.ContainsKey(typeof(T)))
        {
            return false;
        }

        var writer = (Action<IBufferWriter, T>)serializers[typeof(T)];
        writer.Invoke(buffer.Writer, message);
        Sender.Send(new BufferUpdatedEvent(bufferType, buffer.Reader));

        return true;
    }

    protected override async Task<bool> HandleMessageAsync<T>(T message, CancellationToken cancellationToken)
    {
        if (!serializers.ContainsKey(typeof(T)))
        {
            return false;
        }

        var writer = (Action<IBufferWriter, T>)serializers[typeof(T)];
        writer.Invoke(buffer.Writer, message);
        await Sender.SendAsync(new BufferUpdatedEvent(bufferType, buffer.Reader), cancellationToken);

        return true;
    }
}

internal readonly record struct BufferUpdatedEvent(BufferType BufferType, IBufferReader BufferReader);

internal enum BufferType
{
    Receive,
    Transmit
}

public interface ISerializationFilter
{
    void AddSerializer<T>(Action<IBufferWriter, T> serializer);

    void RemoveSerializer<T>();
}
