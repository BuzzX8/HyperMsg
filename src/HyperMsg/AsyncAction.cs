using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Represents asynchronous method without parameters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task that represents async action.</returns>
    public delegate Task AsyncAction(CancellationToken cancellationToken);

    /// <summary>
    /// Represents asynchronous method with one parameters.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    /// <param name="obj">Parameter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task that represents async action.</returns>
    public delegate Task AsyncAction<T>(T obj, CancellationToken cancellationToken);
}
