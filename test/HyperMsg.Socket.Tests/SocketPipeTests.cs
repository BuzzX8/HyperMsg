using FakeItEasy;
using HyperMsg.Sockets;
using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Socket.Tests
{
    public class SocketPipeTests
    {
        private readonly IPipe pipe;
        private readonly ISocket socket;
        private readonly SocketPipe socketPipe;

        public SocketPipeTests()
        {
            pipe = A.Fake<IPipe>();
            socket = A.Fake<ISocket>();
            socketPipe = new SocketPipe(socket);
        }
    }
}
