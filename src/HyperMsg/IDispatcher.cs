namespace HyperMsg;

public interface IDispatcher
{
    void Dispatch<T>(T _);
}