using System.Collections.Concurrent;

namespace HyperMsg;

/// <summary>
/// Provides implementation for MessageSender and MessageHandlerRegistry
/// </summary>
public class MessageBroker : ISender, IRegistry
{
    private readonly ConcurrentDictionary<Type, Delegate> messageHandlers = new();
    private readonly object sync = new();

    public void Send<T>(T data)
    {
        throw new NotImplementedException();
    }

    public void Register<T>(Action<T> dataHandler)
    {
        throw new NotImplementedException();
    }

    public void Deregister<T>(Action<T> handler)
    {
        throw new NotImplementedException();
    }
}
