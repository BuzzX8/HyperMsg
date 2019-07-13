using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal delegate Task InternalDelegates(CancellationToken cancellationToken);

    internal delegate Task MessageHandler<T>(T message, CancellationToken cancellationToken);
}