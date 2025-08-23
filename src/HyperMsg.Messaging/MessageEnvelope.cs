namespace HyperMsg.Messaging;

public class MessageEnvelope<T>
{
    public IDictionary<string, object> Metadata { get; }

    public T Payload { get; }
}