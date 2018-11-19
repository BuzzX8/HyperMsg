using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageChannel<T>
    {
        public MessageChannel(IStream stream, IMessageSerializer<T> serializer, IObserver<T> observer)
        { }

        public IDisposable Run()
        {
            throw new NotImplementedException();
        }

        void Write(T message)
        { }

        Task<FlushResult> FlushAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
