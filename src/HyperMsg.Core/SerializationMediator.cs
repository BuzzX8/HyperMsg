namespace HyperMsg;

public class SerializationMediator : ISender
{
    private readonly IBufferWriter writer;
    private readonly ISerializersRegistry registry;

    public SerializationMediator(IBufferWriter bufferWriter, ISerializersRegistry serializersRegistry)
    {
        writer = bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter));
        registry = serializersRegistry ?? throw new ArgumentNullException(nameof(serializersRegistry));
    }

    public void Send<T>(T message) => SerializeMessage(message);

    public Task SendAsync<T>(T message, CancellationToken _)
    {
        SerializeMessage(message);

        return Task.CompletedTask;
    }

    private void SerializeMessage<T>(in T message)
    {
        if (!TrySerialize(message))
        {
            throw new InvalidOperationException();
        }
    }

    private bool TrySerialize<T>(in T message)
    {
        if (!registry.Contains<T>())
        {
            return false;
        }

        var serializer = registry.Get<T>();

        serializer.Invoke(writer, message);

        return true;
    }
}
