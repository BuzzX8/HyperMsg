using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public delegate Task AsyncAction(CancellationToken cancellationToken);
}
