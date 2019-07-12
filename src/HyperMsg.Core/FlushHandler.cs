using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal delegate Task FlushHandler(CancellationToken cancellationToken);
}
