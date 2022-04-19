namespace HyperMsg;

/// <summary>
/// Defines methods for registering data handlers.
/// </summary>
public interface IRegistry
{    
    void Register<T>(Action<T> dataHandler);

    void Deregister<T>(Action<T> handler);
}