namespace HyperMsg.Messaging;

internal record struct RequestResponse<TRequest, TResponse>(TRequest Request)
{
    internal Action<TResponse> ResponseCallback { get; set; }
}