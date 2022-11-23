namespace HyperMsg;

/// <summary>
/// Registry for data handlers.
/// </summary>
public interface IRegistry
{    
    void Register<T>(Action<T> handler);

    void Unregister<T>(Action<T> handler);
}