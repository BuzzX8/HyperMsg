namespace HyperMsg;

public class SendingPipeline
{
    private readonly ISerializer serializer;
    private readonly IBuffer buffer;

    public void Dispatch<T>(T message)
    {
        serializer.Serialize(buffer.Writer, message);
        OnBufferUpdated();
    }

    private void OnBufferUpdated()
    {
        
    }
}