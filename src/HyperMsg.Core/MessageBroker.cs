using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBroker : IMessageSender, IMessageHandlerRegistry
    {
        public void Register<T>(Action<T> handler)
        {
            throw new NotImplementedException();
        }

        public void Register<T>(AsyncAction<T> handler)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(T message)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}