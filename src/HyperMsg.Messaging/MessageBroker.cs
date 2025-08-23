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

    public void Dispatch<T>(T data) where T : notnull
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
            throw e.InnerException ?? e;
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
            throw e.InnerException ?? e;
        }
    }

    private Task InvokeHandlerAsync<T>(T data, Delegate handler, CancellationToken cancellationToken) where T : notnull => handler switch
    {
        MessageHandler<T> messageHandler => Task.Run(() => messageHandler.Invoke(data), cancellationToken),
        AsyncMessageHandler<T> asyncMessageHandler => asyncMessageHandler.Invoke(data, cancellationToken),
        _ => throw new InvalidOperationException($"Unsupported handler type: {handler.GetType()}")
    };

    public void Register<T>(MessageHandler<T> messageHandler) => Register(typeof(T), messageHandler);

    public void Register<T>(AsyncMessageHandler<T> asyncMessageHandler) => Register(typeof(T), asyncMessageHandler);

    private void Register(Type messageType, Delegate messageHandler)
    {
        lock (sync)
        {
            if (messageHandlers.TryGetValue(messageType, out var handler))
            {
                messageHandlers[messageType] = Delegate.Combine(messageHandler, handler);
            }
            else
            {
                messageHandlers[messageType] = messageHandler;
            }
        }
    }

    public void Unregister<T>(MessageHandler<T> messageHandler) => Unregister(typeof(T), messageHandler);

    public void Unregister<T>(AsyncMessageHandler<T> asyncMessageHandler) => Unregister(typeof(T), asyncMessageHandler);

    private void Unregister(Type messageType, Delegate messageHandler)
    {
        if (!messageHandlers.TryGetValue(messageType, out var handler))
            return;

        lock (sync)
        {
            var source = Delegate.Remove(handler, messageHandler);
            if (source == null)
            {
                messageHandlers.TryRemove(messageType, out _);
            }
            else
            {
                messageHandlers[messageType] = source;
            }
        }
    }

    public void Dispose() => messageHandlers.Clear();
}
