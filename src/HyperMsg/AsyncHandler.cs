using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public delegate Task AsyncHandler<T>(T obj, CancellationToken cancellationToken);
}
