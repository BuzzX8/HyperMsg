using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    internal class SocketProxy : ISocket, ISupportsTls, IDisposable
    {
        private readonly Socket socket;        
        private readonly EndPoint endpoint;
        private Stream stream;

        public SocketProxy(Socket socket, EndPoint endpoint)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.endpoint = endpoint;
        }

        public bool ValidateAllCertificates { get; }

        public Stream Stream => GetStream();

        public bool IsConnected => socket.Connected;

        public void Connect() => socket.Connect(endpoint);

        public Task ConnectAsync(CancellationToken token) => Task.Run((Action)Connect);

        public void Disconnect() => socket.Disconnect(false);

        public Task DisconnectAsync(CancellationToken token) => Task.Run((Action)Disconnect);

        public void Dispose() => Disconnect();

        public void SetTls()
        {
            if (stream == null)
            {
                throw new InvalidOperationException();
            }

            if (stream is SslStream)
            {
                return;
            }
            
            var sslStream = new SslStream(stream, false, ValidateRemoteCertificate);
            sslStream.AuthenticateAsClient(endpoint.ToString());
            stream = sslStream;
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (ValidateAllCertificates)
            {
                return true;
            }

            var eventArgs = new RemoteCertificateValidationEventArgs(certificate, chain, sslPolicyErrors);
            RemoteCertificateValidationRequired?.Invoke(this, eventArgs);

            return eventArgs.IsValid;
        }

        private Stream GetStream()
        {
            if (stream == null)
            {
                stream = new NetworkStream(socket);
            }

            return stream;
        }

        public event EventHandler<RemoteCertificateValidationEventArgs> RemoteCertificateValidationRequired;
    }
}
