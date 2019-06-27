using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITransport
    {
        IStream GetStream();

        Task ProcessCommandAsync(TransportCommand command, CancellationToken cancellationToken);

        event EventHandler<TransportEventArgs> TransportEvent;
    }
}