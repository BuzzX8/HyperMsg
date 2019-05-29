using FakeItEasy;
using HyperMsg.Sockets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Socket.Tests
{
    public class SocketExtensionTests
    {
        private readonly byte[] bytes;
        private readonly MemoryStream stream;
        private readonly ISocket socket;
        private readonly CancellationToken token;

        public SocketExtensionTests()
        {
            bytes = Guid.NewGuid().ToByteArray();
            stream = new MemoryStream(bytes);
            socket = A.Fake<ISocket>();
            token = new CancellationToken();
            A.CallTo(() => socket.Stream).Returns(stream);
        }

        [Fact]
        public void Read_Reads_Bytes_From_Stream_Into_Buffer()
        {            
            var actualBytes = new byte[bytes.Length];

            int readed = socket.Read(actualBytes);

            Assert.Equal(bytes.Length, readed);
            Assert.Equal(bytes, actualBytes);
        }

        [Fact]
        public async Task ReadAsync_Reads_Bytes_From_Stream_Into_Buffer()
        {
            var actualBytes = new byte[bytes.Length];

            int readed = await socket.ReadAsync(actualBytes, token);

            Assert.Equal(bytes.Length, readed);
            Assert.Equal(bytes, actualBytes);
        }

        [Fact]
        public void Write_Writes_Bytes_Into_Stream()
        {
            socket.Write(bytes);

            var actualBytes = stream.ToArray();

            Assert.Equal(bytes, actualBytes);
        }

        [Fact]
        public async Task WriteAsync_Writes_Bytes_Into_Stream()
        {
            await socket.WriteAsync(bytes, token);
                        
            Assert.Equal(bytes, stream.ToArray());
        }
    }
}
