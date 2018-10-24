using FakeItEasy;
using HyperMsg.Sockets;
using System;
using System.IO;
using Xunit;

namespace HyperMsg.Socket.Tests
{
    public class SocketExtensionTests
    {
        [Fact]
        public void Read_Reads_Bytes_From_Stream_Into_Buffer()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream(bytes);
            var socket = A.Fake<ISocket>();
            A.CallTo(() => socket.Stream).Returns(stream);
            var actualBytes = new byte[bytes.Length];

            int readed = socket.Read(actualBytes);

            Assert.Equal(bytes.Length, readed);
            Assert.Equal(bytes, actualBytes);
        }
    }
}
