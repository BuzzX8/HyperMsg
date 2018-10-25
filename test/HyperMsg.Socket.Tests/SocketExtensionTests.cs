using FakeItEasy;
using HyperMsg.Sockets;
using System;
using System.IO;
using System.Threading.Tasks;
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

        [Fact]
        public async Task ReadAsync_Reads_Bytes_From_Stream_Into_Buffer()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream(bytes);
            var socket = A.Fake<ISocket>();
            A.CallTo(() => socket.Stream).Returns(stream);
            var actualBytes = new byte[bytes.Length];

            int readed = await socket.ReadAsync(actualBytes);

            Assert.Equal(bytes.Length, readed);
            Assert.Equal(bytes, actualBytes);
        }

        //[Fact]
        public void Write_Writes_Bytes_Into_Stream()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream();
            var socket = A.Fake<ISocket>();
            A.CallTo(() => socket.Stream).Returns(stream);
            
            socket.Write(bytes);

            var actualBytes = stream.ToArray();

            Assert.Equal(bytes, actualBytes);
        }

        [Fact]
        public async Task WriteAsync_Writes_Bytes_Into_Stream()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream();
            var socket = A.Fake<ISocket>();
            A.CallTo(() => socket.Stream).Returns(stream);

            await socket.WriteAsync(bytes);
                        
            Assert.Equal(bytes, stream.ToArray());
        }
    }
}
