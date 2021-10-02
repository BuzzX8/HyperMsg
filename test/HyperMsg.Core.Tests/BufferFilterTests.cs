using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferFilterTests : HostFixture
    {
        [Fact]
        public void Send_Writes_Message_To_Buffer()
        {
            var message = Guid.NewGuid();
            var bytes = default(byte[]);
            var filter = GetRequiredService<BufferFilter>();

            filter.AddWriter<Guid>((writer, guid) => writer.Write(guid.ToByteArray()));
            HandlersRegistry.RegisterTransmitBufferHandler(reader => bytes = reader.Read().ToArray());

            filter.Send(message);

            Assert.Equal(message.ToByteArray(), bytes);
        }

        [Fact]
        public async Task SendAsync_Writes_Message_To_Buffer()
        {
            var message = Guid.NewGuid();
            var bytes = default(byte[]);
            var filter = GetRequiredService<BufferFilter>();

            filter.AddWriter<Guid>((writer, guid) => writer.Write(guid.ToByteArray()));
            HandlersRegistry.RegisterTransmitBufferHandler((reader, _) =>
            {
                bytes = reader.Read().ToArray();
                return Task.CompletedTask;
            });

            await filter.SendAsync(message, default);

            Assert.Equal(message.ToByteArray(), bytes);
        }
    }
}