namespace HyperMsg;

public class SendBufferFilter
{
    public void Send(IBuffer buffer)
    {
        var reader = buffer.Reader;
        
        var bytesSent = SendBufferData?.Invoke(reader);

        if (bytesSent != null)
        {
            reader.Advance(bytesSent.Value);
        }
    }

    public event Func<IBufferReader, int> SendBufferData;
}