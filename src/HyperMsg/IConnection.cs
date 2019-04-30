using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IConnection
    {
        ConnectionState State { get; }

        void Open();

        Task OpenAsync(CancellationToken token = default);

        void Close();

        Task CloseAsync(CancellationToken token = default);

        event EventHandler<ConnectionStateChangedEventArgs> StateChanged;
    }
}
