using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class ConnectionListener
    {
        private readonly Lazy<Socket> socket;
        private readonly EndPoint endpoint;

        public ConnectionListener(Func<Socket> socketFactory, EndPoint endpoint)
        {
            socket = new Lazy<Socket>(socketFactory);
            this.endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        protected Socket Socket => socket.Value;

        protected async Task DoWorkAsync(CancellationToken token)
        {
            
        }
    }
}
