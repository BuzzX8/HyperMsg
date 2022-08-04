namespace HyperMsg;

public class SendingPipeline : IDispatcher
{
    private readonly ISerializer serializer;
    private readonly IBuffer buffer;

    public SendingPipeline(ISerializer serializer, IBuffer buffer)
    {
        this.serializer = serializer;
        this.buffer = buffer;
    }

    public Action<IBufferReader> BufferHandler { get; set; }

    public void Dispatch<T>(T message)
    {
        serializer.Serialize(buffer.Writer, message);
        OnBufferUpdated();
    }

    private void OnBufferUpdated() => BufferHandler?.Invoke(buffer.Reader);
}