using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class ConnectionListener : ObservableListener<SocketProxy>
    {
        private readonly Lazy<Socket> socket;
        private readonly EndPoint endpoint;

        public ConnectionListener(Func<Socket> socketFactory, EndPoint endpoint, IObserver<SocketProxy> observer) : base(observer)
        {
            socket = new Lazy<Socket>(socketFactory);
            this.endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        protected Socket Socket => socket.Value;

        protected override async Task DoListening(CancellationToken token)
        {
            Socket.Bind(endpoint);
            Socket.Listen(1);
            while(!token.IsCancellationRequested)
            {
                var result = await Task.Run(() => Socket.Accept());
                OnNext(new SocketProxy(result));
            }
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            Socket.Close();
        }
    }
}
