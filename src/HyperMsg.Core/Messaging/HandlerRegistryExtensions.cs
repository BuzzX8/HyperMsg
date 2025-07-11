namespace HyperMsg.Messaging;

public static class HandlerRegistryExtensions
{
    public static void RegisterRequestHandler<TRequest, TResponse>(this IHandlerRegistry registry, RequestHandler<TRequest, TResponse> handler)
        => registry.Register<RequestResponse<TRequest, TResponse>>(message => message.Response = handler(message.Request));

    public static void RegisterRequestHandler<TRequest, TResponse>(this IHandlerRegistry registry, AsyncRequestHandler<TRequest, TResponse> handler) 
        => registry.Register<RequestResponse<TRequest, TResponse>>(async message => message.Response = await handler(message.Request));
}

public delegate TResponse RequestHandler<TRequest, TResponse>(TRequest request);

public delegate Task<TResponse> AsyncRequestHandler<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);