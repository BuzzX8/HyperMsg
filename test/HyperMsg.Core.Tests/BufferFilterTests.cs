using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferFilterTests : HostFixture
    {
        private readonly IBufferFilter bufferFilter;

        public BufferFilterTests()
        {
            bufferFilter = GetRequiredService<IBufferFilter>();
        }

        [Fact]
        public void Send_Writes_Message_To_Buffer()
        {
            var message = Guid.NewGuid();
            var bytes = default(byte[]);

            bufferFilter.AddWriter<Guid>((writer, guid) => writer.Write(guid.ToByteArray()));
            HandlersRegistry.RegisterTransmitBufferHandler(reader => bytes = reader.Read().ToArray());

            Sender.Send(message);

            Assert.Equal(message.ToByteArray(), bytes);
        }

        [Fact]
        public async Task SendAsync_Writes_Message_To_Buffer()
        {
            var message = Guid.NewGuid();
            var bytes = default(byte[]);

            bufferFilter.AddWriter<Guid>((writer, guid) => writer.Write(guid.ToByteArray()));
            HandlersRegistry.RegisterTransmitBufferHandler((reader, _) =>
            {
                bytes = reader.Read().ToArray();
                return Task.CompletedTask;
            });

            await Sender.SendAsync(message, default);

            Assert.Equal(message.ToByteArray(), bytes);
        }
    }
}