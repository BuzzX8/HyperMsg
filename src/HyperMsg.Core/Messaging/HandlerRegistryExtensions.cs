namespace HyperMsg.Messaging;

/// <summary>
/// Provides extension methods for registering message and request handlers with an <see cref="IHandlerRegistry"/>.
/// </summary>
public static class HandlerRegistryExtensions
{
    /// <summary>
    /// Registers a synchronous message handler for messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The message handler to register.</param>
    /// <returns>An <see cref="IDisposable"/> that can be used to unregister the handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
    public static IDisposable RegisterHandler<T>(this IHandlerRegistry registry, MessageHandler<T> handler)
    {
        throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// Registers an asynchronous message handler for messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The asynchronous message handler to register.</param>
    /// <returns>An <see cref="IDisposable"/> that can be used to unregister the handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
    public static IDisposable RegisterHandler<T>(this IHandlerRegistry registry, AsyncMessageHandler<T> handler)
    {
        throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// Registers a synchronous request handler for requests of type <typeparamref name="TRequest"/> and responses of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The request handler to register.</param>
    public static void RegisterRequestHandler<TRequest, TResponse>(this IHandlerRegistry registry, RequestHandler<TRequest, TResponse> handler)
        => registry.Register<RequestResponse<TRequest, TResponse>>(message => message.ResponseCallback(handler(message.Request)));

    /// <summary>
    /// Registers an asynchronous request handler for requests of type <typeparamref name="TRequest"/> and responses of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="registry">The handler registry to register the handler with.</param>
    /// <param name="handler">The asynchronous request handler to register.</param>
    public static void RegisterRequestHandler<TRequest, TResponse>(this IHandlerRegistry registry, AsyncRequestHandler<TRequest, TResponse> handler) 
        => registry.Register<RequestResponse<TRequest, TResponse>>(async message => message.ResponseCallback(await handler(message.Request)));
}

/// <summary>
/// Represents a delegate for handling requests synchronously.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="request">The request to handle.</param>
/// <returns>The response to the request.</returns>
public delegate TResponse RequestHandler<TRequest, TResponse>(TRequest request);

/// <summary>
/// Represents a delegate for handling requests asynchronously.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="request">The request to handle.</param>
/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
/// <returns>A task that represents the asynchronous operation, containing the response to the request.</returns>
public delegate Task<TResponse> AsyncRequestHandler<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);