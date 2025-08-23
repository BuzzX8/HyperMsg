namespace HyperMsg.Messaging;

public static class DispatcherExtensions
{
    public static TResponse? DispatchRequest<TRequest, TResponse>(this IDispatcher dispatcher, TRequest request)
    {
        var message = new RequestResponse<TRequest, TResponse>(request);
        var response = default(TResponse);

        message.ResponseCallback = result => response = result;
        dispatcher.Dispatch(message);

        return response;
    }

    public static async Task<TResponse?> DispatchRequestAsync<TRequest, TResponse>(this IDispatcher dispatcher, TRequest request, CancellationToken cancellationToken = default)
    {
        var message = new RequestResponse<TRequest, TResponse>(request);
        var response = default(TResponse);

        message.ResponseCallback = result => response = result;
        await dispatcher.DispatchAsync(message, cancellationToken);

        return response;
    }
}
