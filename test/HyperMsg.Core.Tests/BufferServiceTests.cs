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
        public void SendInvokeReceiveBufferHandlersCommand_Invokes_Handler_Registered_With_RegisterReceiveBufferHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterReceiveBufferHandler(buffer => actualData = buffer.Reader.Read().ToArray());

            Sender.SendInvokeReceiveBufferHandlersCommand();

            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public async Task SendInvokeReceiveBufferHandlersCommandAsync_Invokes_Handler_Registered_With_RegisterReceiveBufferHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterReceiveBufferHandler((buffer, _) =>
            {
                actualData = buffer.Reader.Read().ToArray();
                return Task.CompletedTask;
            });

            await Sender.SendInvokeReceiveBufferHandlersCommandAsync();

            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public void SendInvokeTransmitBufferHandlersCommand_Invokes_Handler_Registered_With_RegisterTransmitBufferHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterTransmitBufferHandler(buffer => actualData = buffer.Reader.Read().ToArray());

            Sender.SendInvokeTransmitBufferHandlersCommand();

            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public async Task SendInvokeTransmitBufferHandlersCommandAsync_Invokes_Handler_Registered_With_RegisterTransmitBufferHandler()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            buffer.Writer.Write(expectedData);
            HandlersRegistry.RegisterTransmitBufferHandler((buffer, _) =>
            {
                actualData = buffer.Reader.Read().ToArray();
                return Task.CompletedTask;
            });

            await Sender.SendInvokeTransmitBufferHandlersCommandAsync();

            Assert.Equal(expectedData, actualData);
        }
    }
}