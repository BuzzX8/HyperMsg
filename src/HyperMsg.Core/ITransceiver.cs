using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITransceiver<in TSend, TReceive> : ISender<TSend>, IReceiver<TReceive>
    { }
}