namespace HyperMsg.Messaging;

public static class DispatcherExtensions
{
    public static TResponse DispatchRequest<TRequest, TResponse>(this IDispatcher dispatcher, TRequest request)
    {
        var message = new RequestResponse<TRequest, TResponse>(request, default);
        dispatcher.Dispatch(message);
        return message.Response;
    }

    public static async Task<TResponse> DispatchRequestAsync<TRequest, TResponse>(this IDispatcher dispatcher, TRequest request, CancellationToken cancellationToken = default)
    {
        var message = new RequestResponse<TRequest, TResponse>(request, default);
        await dispatcher.DispatchAsync(message, cancellationToken);
        return message.Response;
    }
}
