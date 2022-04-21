namespace HyperMsg;

public interface IForwarder
{
    void Dispatch<T>(T _);
}