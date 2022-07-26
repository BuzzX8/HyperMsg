namespace HyperMsg;

public record BufferUpdatedEvent(IBuffer Buffer, BufferType BufferType = BufferType.None);

public enum BufferType
{
    None,
    Receiving,
    Transmitting
}
