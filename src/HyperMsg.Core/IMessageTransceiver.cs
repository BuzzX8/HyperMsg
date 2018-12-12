using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessageTransceiver<T> : IObservable<T>
    {
        IDisposable Run();
        void Send(T message);
        Task SendAsync(T message, CancellationToken token = default);        
    }
}
