namespace HyperMsg
{
    public interface IRuntime
    {
        IBroker SendingBroker { get; }

        IBroker ReceivingBroker { get; }

        ISerializersRegistry SerializersRegistry { get; }
    }
}
