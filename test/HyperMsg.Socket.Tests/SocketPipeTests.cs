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
            socketPipe = new SocketPipe(pipe, socket);
        }

        [Fact]
        public async Task TransferFromSocketAsync_Reads_Data_From_Socket_And_Writes_Into_Writer()
        {
            var data = Guid.NewGuid().ToByteArray();
            var buffer = new byte[data.Length];
            A.CallTo(() => pipe.Writer.GetMemory(A<int>._)).Returns(new Memory<byte>(buffer));
            A.CallTo(() => socket.Stream).Returns(new MemoryStream(data));

            await socketPipe.TransferFromSocketAsync(default);

            Assert.Equal(data, buffer);
        }

        [Fact]
        public async Task TransferToSocketAsync_Reads_Data_From_Reader_And_Writes_Into_Socket()
        {
            var data = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream();
            A.CallTo(() => socket.Stream).Returns(stream);
            A.CallTo(() => pipe.Reader.ReadAsync(default)).Returns(Task.FromResult(new ReadOnlySequence<byte>(data)));

            await socketPipe.TransferToSocketAsync(default);

            Assert.Equal(data, stream.ToArray());
        }
    }
}
