namespace HyperMsg.Messaging;

internal record struct RequestResponse<TRequest, TResponse>(TRequest Request, TResponse Response);