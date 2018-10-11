using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class ConnectionListener : ObservableListener<SocketProxy>
    {
        private Lazy<Socket> socket;

        public ConnectionListener(Func<Socket> socketFactory, IObserver<SocketProxy> observer) : base(observer)
        {
            socket = new Lazy<Socket>(socketFactory);
        }

        protected Socket Socket => socket.Value;

        protected override async Task DoListening(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                var result = await Task.Run(() => Socket.Accept());
                OnNext(new SocketProxy(result));
            }
        }
    }
}
