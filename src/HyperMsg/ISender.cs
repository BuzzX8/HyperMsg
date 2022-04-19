namespace HyperMsg;

public interface ISender
{
    void Send<T>(T data);
}