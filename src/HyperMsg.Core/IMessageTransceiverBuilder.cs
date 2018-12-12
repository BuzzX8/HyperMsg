namespace HyperMsg
{
    public interface IMessageTransceiverBuilder<T>
    {
        IMessageTransceiver<T> Build();
    }
}
