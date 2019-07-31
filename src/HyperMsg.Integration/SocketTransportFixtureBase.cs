using HyperMsg.Sockets;
using System.Net;

namespace HyperMsg.Integration
{
    public abstract class SocketTransportFixtureBase<T> : IntegrationFixtureBase<T>
    {
        protected SocketTransportFixtureBase(int port) : this(IPAddress.Loopback, port)
        { }

        protected SocketTransportFixtureBase(IPAddress address, int port)
        {
            EndPoint = new IPEndPoint(address, port);            
        }

        protected EndPoint EndPoint { get; }

        protected override void ConfigureTransport(IConfigurable configurable) => configurable.UseSockets(EndPoint);
    }
}
