using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public class ConnectionListener// : BackgroundWorker
    {
        private Socket listeningSocket;

        public void StartListening(EndPoint endPoint, int backlog = 1)
        {
            listeningSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(endPoint);
            listeningSocket.Listen(backlog);
            //Run();
        }

        public void StopListening()
        {
            listeningSocket.Close();
            //Stop();            
        }

        //protected override async Task DoWorkIterationAsync(CancellationToken cancellationToken)
        //{
        //    var acceptedSocket = await Task.Run(() => listeningSocket.Accept(), cancellationToken);
        //    ConnectionAccepted?.Invoke(acceptedSocket);
        //}

        public Action<Socket> ConnectionAccepted;
    }
}
