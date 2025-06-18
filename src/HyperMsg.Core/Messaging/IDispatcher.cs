namespace HyperMsg.Messaging;

public interface IDispatcher
{
    void Dispatch<T>(T data) where T : notnull;

    Task DispatchAsync<T>(T data, CancellationToken cancellationToken = default) where T : notnull;
}
