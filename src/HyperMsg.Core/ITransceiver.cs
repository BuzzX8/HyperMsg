using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITransceiver<in TSend, out TReceive> : IObservable<TReceive>
    {
        IDisposable Run();
        void Send(TSend message);
        Task SendAsync(TSend message, CancellationToken token = default);        
    }
}