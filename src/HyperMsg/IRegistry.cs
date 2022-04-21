namespace HyperMsg;

/// <summary>
/// Registry for data handlers.
/// </summary>
public interface IRegistry
{    
    void Register<T>(Action<T> handler);

    void Deregister<T>(Action<T> handler);
}