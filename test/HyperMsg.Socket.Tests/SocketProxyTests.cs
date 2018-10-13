using HyperMsg.Sockets;
using System;
using System.Net;
using System.Reactive;
using System.Threading;
using Xunit;

namespace HyperMsg.Socket.Tests
{
    public class SocketProxyTests : IDisposable
    {
        private readonly IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 8080);
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        private ConnectionListener listener;
        private readonly SocketProxy socket;
        private readonly ManualResetEventSlim @event;

        public SocketProxyTests()
        {
            socket = new SocketProxy(CreateSocket, endpoint);
            @event = new ManualResetEventSlim();
        }

        [Fact]
        public void Connect_Establishes_Connection()
        {            
            var acceptedSocket = (SocketProxy)null;
            var listener = CreateListener(s =>
            {
                acceptedSocket = s;
                @event.Set();
            });
            listener.Start();
            
            socket.Connect();
            @event.Wait(waitTimeout);

            Assert.NotNull(acceptedSocket);
        }

        [Fact]
        public void Can_Send_Data_Over_Stream()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = new byte[expected.Length];
            var acceptedSocket = (SocketProxy)null;
            var listener = CreateListener(s =>
            {
                acceptedSocket = s;
                @event.Set();
            });
            listener.Start();

            socket.Connect();
            @event.Wait(waitTimeout);

            socket.Stream.Write(expected);
            int readed = acceptedSocket.Stream.Read(actual);

            Assert.Equal(expected.Length, readed);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_Receive_Data_Over_Stream()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = new byte[expected.Length];
            var acceptedSocket = (SocketProxy)null;
            var listener = CreateListener(s =>
            {
                acceptedSocket = s;
                @event.Set();
            });
            listener.Start();

            socket.Connect();
            @event.Wait(waitTimeout);

            acceptedSocket.Stream.Write(expected);
            int readed = socket.Stream.Read(actual);

            Assert.Equal(expected.Length, readed);
            Assert.Equal(expected, actual);
        }

        private System.Net.Sockets.Socket CreateSocket()
        {
            return new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
        }

        private ConnectionListener CreateListener(Action<SocketProxy> onNext)
        {
            var observer = Observer.Create(onNext);
            listener = new ConnectionListener(CreateSocket, endpoint, observer);
            return listener;
        }

        public void Dispose()
        {
            listener.Stop();
            socket.Dispose();            
        }
    }
}
