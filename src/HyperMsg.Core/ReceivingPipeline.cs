namespace HyperMsg;

public class ReceivingPipeline
{
    private readonly IDispatcher dispatcher;

    public ReceivingPipeline(IDispatcher dispatcher) => this.dispatcher = dispatcher;

    public Action<IBufferReader, IDispatcher> Deserializer { get; set; }

    public void OnBufferUpdated(IBuffer buffer)
    {
        Deserializer.Invoke(buffer.Reader, dispatcher);
    }
}
