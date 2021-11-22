namespace HyperMsg;

/// <summary>
/// Defines methods for sending data.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends data.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <param name="data">Data to send.</param>
    void Send<T>(T data);

    /// <summary>
    /// Sends data asynchronous.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <param name="data">Data to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task that represents async status of send operation.</returns>
    Task SendAsync<T>(T data, CancellationToken cancellationToken);
}