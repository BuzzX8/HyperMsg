using System.Collections.Concurrent;
using System.Reflection;

namespace HyperMsg.Messaging;

/// <summary>
/// Provides implementation for MessageSender and MessageHandlerRegistry
/// </summary>
public class MessageBroker
{
    private readonly ConcurrentDictionary<Type, Delegate> messageHandlers = new();
    private readonly object sync = new();

    public void Dispatch<T>(T data)
    {
        if (!messageHandlers.ContainsKey(typeof(T)))
        {
            return;
        }

        if (!messageHandlers.TryGetValue(typeof(T), out var handler))
        {
            return;
        }

        try
        {
            ((Action<T>)handler).Invoke(data);
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    public void Register<T>(Action<T> messageHandler)
    {
        lock (sync)
        {
            if (messageHandlers.TryGetValue(typeof(T), out var handler))
            {
                messageHandlers[typeof(T)] = Delegate.Combine(messageHandler, handler);
            }
            else
            {
                messageHandlers[typeof(T)] = messageHandler;
            }
        }
    }

    public void Unregister<T>(Action<T> messageHandler)
    {
        lock (sync)
        {
            var source = Delegate.Remove(messageHandlers[typeof(T)], messageHandler);

            if (source == null)
            {
                messageHandlers.TryRemove(typeof(T), out var value);
                return;
            }
            else
            {
                messageHandlers[typeof(T)] = source;
            }
        }
    }
}
