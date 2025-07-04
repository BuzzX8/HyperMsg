using System.Collections.Concurrent;
using System.Reflection;

namespace HyperMsg.Messaging;

/// <summary>
/// Provides implementation for MessageSender and MessageHandlerRegistry
/// </summary>
public class MessageBroker : IDispatcher, IHandlerRegistry, IMessagingContext, IDisposable
{
    private readonly ConcurrentDictionary<Type, Delegate> messageHandlers = new();
    private readonly object sync = new();

    public IDispatcher Dispatcher => this;

    public IHandlerRegistry HandlerRegistry => this;

    public void Dispatch<T>(T data)
    {
        if (!messageHandlers.ContainsKey(typeof(T)))
        {
            return;
        }

        if (!messageHandlers.TryGetValue(typeof(T), out var handlers))
        {
            return;
        }

        try
        {
            foreach (var handler in handlers.GetInvocationList())
            {
                InvokeHandler(data, handler);
            }
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    private static void InvokeHandler<T>(T data, Delegate handler)
    {
        switch (handler)
        {
            case MessageHandler<T> messageHandler:
                messageHandler.Invoke(data);
                break;
            case AsyncMessageHandler<T> asyncMessageHandler:
                asyncMessageHandler.Invoke(data, CancellationToken.None).GetAwaiter().GetResult();
                break;
            default:
                throw new InvalidOperationException($"Unsupported handler type: {handler.GetType()}");
        }
    }

    public async Task DispatchAsync<T>(T data, CancellationToken cancellationToken = default) where T : notnull
    {
        if (!messageHandlers.ContainsKey(typeof(T)))
        {
            return;
        }

        if (!messageHandlers.TryGetValue(typeof(T), out var handlers))
        {
            return;
        }

        try
        {
            foreach (var handler in handlers.GetInvocationList())
            {
                await InvokeHandlerAsync(data, handler, cancellationToken);
            }
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    private Task InvokeHandlerAsync<T>(T data, Delegate handler, CancellationToken cancellationToken) where T : notnull
    {
        return handler switch
        {
            MessageHandler<T> messageHandler => Task.Run(() => messageHandler.Invoke(data), cancellationToken),
            AsyncMessageHandler<T> asyncMessageHandler => asyncMessageHandler.Invoke(data, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported handler type: {handler.GetType()}")
        };
    }

    public void Register<T>(MessageHandler<T> messageHandler)
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

    public void Register<T>(AsyncMessageHandler<T> asyncMessageHandler)
    {
        throw new NotImplementedException("Async message handler registration is not implemented yet.");
    }

    public void Unregister<T>(MessageHandler<T> messageHandler)
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

    public void Unregister<T>(AsyncMessageHandler<T> asyncMessageHandler)
    {
        throw new NotImplementedException("Async message handler unregistration is not implemented yet.");
    }

    public void Dispose() => messageHandlers.Clear();
}
