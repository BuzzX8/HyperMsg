using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITransport
    {
        Task ProcessCommandAsync(TransportCommand command, CancellationToken cancellationToken);

        event AsyncAction<TransportEventArgs> TransportEvent;
    }
}