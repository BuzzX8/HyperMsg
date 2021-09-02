using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : HostFixture
    {
        [Fact]
        public void SendActionRequestToReceiveBuffer_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            Sender.SendActionRequestToReceiveBuffer(buffer => buffer.Writer.Write(expectedData));

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SendActionRequestToReceiveBufferAsync_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            await Sender.SendActionRequestToReceiveBufferAsync((buffer, _) =>
            {
                buffer.Writer.Write(expectedData);
                return Task.CompletedTask;
            });

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public void SendActionRequestToTransmitBuffer_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            Sender.SendActionRequestToTransmitBuffer(buffer => buffer.Writer.Write(expectedData));

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SendActionRequestToTransmitBufferAsync_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            await Sender.SendActionRequestToTransmitBufferAsync((buffer, _) =>
            {
                buffer.Writer.Write(expectedData);
                return Task.CompletedTask;
            });

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public void SendHandleReceiveBufferCommand_Invokes_Handler_Registered_With_RegisterReceiveBufferCommandHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterReceiveBufferCommandHandler(buffer => actualData = buffer.Reader.Read().ToArray());

            Sender.SendHandleReceiveBufferCommand();

            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public async Task SendHandleReceiveBufferCommandAsync_Invokes_Handler_Registered_With_RegisterReceiveBufferCommandHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterReceiveBufferCommandHandler((buffer, _) =>
            {
                actualData = buffer.Reader.Read().ToArray();
                return Task.CompletedTask;
            });

            await Sender.SendHandleReceiveBufferCommandAsync();

            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public void SendHandleTransmitBufferCommand_Invokes_Handler_Registered_With_RegisterReceiveBufferCommandHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterTransmitBufferCommandHandler(buffer => actualData = buffer.Reader.Read().ToArray());

            Sender.SendHandleTransmitBufferCommand();

            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public async Task SendHandleTransmitBufferCommandAsync_Invokes_Handler_Registered_With_RegisterReceiveBufferCommandHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterTransmitBufferCommandHandler((buffer, _) =>
            {
                actualData = buffer.Reader.Read().ToArray();
                return Task.CompletedTask;
            });

            await Sender.SendHandleTransmitBufferCommandAsync();

            Assert.Equal(expectedData, actualData);
        }
    }
}