namespace HyperMsg.Buffers;

public interface IBufferingComponent
{
    void Attach(IBufferingContext context);

    void Detach(IBufferingContext context);
}
