using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HyperMsg.Sockets
{
    public class RemoteCertificateValidationEventArgs : EventArgs
    {
        public RemoteCertificateValidationEventArgs(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
            Chain = chain ?? throw new ArgumentNullException(nameof(chain));
            SslPolicyErrors = sslPolicyErrors;
        }

        public X509Certificate Certificate { get; }

        public X509Chain Chain { get; }

        public SslPolicyErrors SslPolicyErrors { get; }

        public bool IsValid { get; set; }
    }
}
