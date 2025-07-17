namespace HyperMsg.Messaging;

/// <summary>
/// Provides extension methods for registering and unregistering message and request handlers in an <see cref="IHandlerRegistry"/>.
/// </summary>
public static class HandlerRegistryExtensions
{
    /// <summary>
    /// Registers a synchronous message handler for messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The message handler to register.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that, when disposed, unregisters the handler from the registry.
    /// </returns>
    public static IDisposable RegisterHandler<T>(this IHandlerRegistry registry, MessageHandler<T> handler)
    {
        registry.Register(handler);

        return new HandlerRegistration(() => registry.Unregister(handler));
    }

    /// <summary>
    /// Registers an asynchronous message handler for messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The asynchronous message handler to register.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that, when disposed, unregisters the handler from the registry.
    /// </returns>
    public static IDisposable RegisterHandler<T>(this IHandlerRegistry registry, AsyncMessageHandler<T> handler)
    {
        registry.Register(handler);

        return new HandlerRegistration(() => registry.Unregister(handler));
    }

    /// <summary>
    /// Registers a synchronous request handler for requests of type <typeparamref name="TRequest"/> and responses of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message.</typeparam>
    /// <typeparam name="TResponse">The type of the response message.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The request handler to register.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that, when disposed, unregisters the handler from the registry.
    /// </returns>
    public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IHandlerRegistry registry, RequestHandler<TRequest, TResponse> handler)
        => registry.RegisterHandler<RequestResponse<TRequest, TResponse>>(message => message.ResponseCallback(handler(message.Request)));

    /// <summary>
    /// Registers an asynchronous request handler for requests of type <typeparamref name="TRequest"/> and responses of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request message.</typeparam>
    /// <typeparam name="TResponse">The type of the response message.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The asynchronous request handler to register.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that, when disposed, unregisters the handler from the registry.
    /// </returns>
    public static IDisposable RegisterRequestHandler<TRequest, TResponse>(this IHandlerRegistry registry, AsyncRequestHandler<TRequest, TResponse> handler) 
        => registry.RegisterHandler<RequestResponse<TRequest, TResponse>>(async message => message.ResponseCallback(await handler(message.Request)));
}

/// <summary>
/// Represents a handler for processing a request of type <typeparamref name="TRequest"/> and returning a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="request">The request to process.</param>
/// <returns>The response to the request.</returns>
public delegate TResponse RequestHandler<TRequest, TResponse>(TRequest request);

/// <summary>
/// Represents an asynchronous handler for processing a request of type <typeparamref name="TRequest"/> and returning a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="request">The request to process.</param>
/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
/// <returns>A task that represents the asynchronous operation, containing the response to the request.</returns>
public delegate Task<TResponse> AsyncRequestHandler<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);