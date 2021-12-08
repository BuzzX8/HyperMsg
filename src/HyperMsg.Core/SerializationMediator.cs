namespace HyperMsg;

public class SerializationMediator : ISender
{
    private readonly IBuffer buffer;
    private readonly ISerializersRegistry registry;

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

        serializer.Invoke(buffer.Writer, message);

        return true;
    }
}
