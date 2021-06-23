using FakeItEasy;
using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : ServiceHostFixture
    {
        [Fact]
        public void SendToTransmitBuffer_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Transmit, bufferReader);
            MessageSender.SendToTransmitBuffer(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTransmitBufferAsync_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Transmit, bufferReader);
            await MessageSender.SendToTransmitBufferAsync(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToTransmitBuffer_Does_Not_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Transmit, bufferReader);
            MessageSender.SendToTransmitBuffer(Guid.NewGuid().ToByteArray(), false);

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendToReceiveBuffer_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Receive, bufferReader);
            MessageSender.SendToReceiveBuffer(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToReceiveBufferAsync_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Receive, bufferReader);
            await MessageSender.SendToReceiveBufferAsync(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToReceiveBuffer_Does_Not_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Receive, bufferReader);
            MessageSender.SendToReceiveBuffer(Guid.NewGuid().ToByteArray(), false);

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendTransmitToBuffer_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmit, reader => actual = reader.Read().ToArray());
            MessageSender.SendToTransmitBuffer(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SendTransmitToBufferAsync_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmit, reader => actual = reader.Read().ToArray());
            await MessageSender.SendToTransmitBufferAsync(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendReceiveToBuffer_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Receive, reader => actual = reader.Read().ToArray());
            MessageSender.SendToReceiveBuffer(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SendReceiveToBufferAsync_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Receive, reader => actual = reader.Read().ToArray());
            await MessageSender.SendToReceiveBufferAsync(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendFlushBufferCommand_Invokes_Handler_Registered_By_RegisterBufferFlushSegmentReader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmit, reader => actual = reader.Read().ToArray());
            MessageSender.SendToTransmitBuffer(expected);

            MessageSender.SendFlushBufferCommand(PipeType.Transmit);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendFlushBufferCommand_Invokes_Async_Handler_Registered_By_RegisterBufferFlushSegmentReader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmit, (buffer, _) =>
            {
                actual = buffer.Read().ToArray();
                return Task.CompletedTask;
            });
            MessageSender.SendToTransmitBuffer(expected);

            MessageSender.SendFlushBufferCommand(PipeType.Transmit);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RegisterBufferFlushDataHandler_Advances_Buffer_Reader()
        {
            var data = Guid.NewGuid().ToByteArray();
            var bufferCapacity = GetRequiredService<IBufferContext>().TransmittingBuffer.Writer.GetMemory().Length;
            var iterationCount = (bufferCapacity / data.Length) * 10;
            HandlersRegistry.RegisterTransmitPipeHandler<IBufferReader>(data => { });

            for (int i = 0; i < iterationCount; i++)
            {
                try
                {
                    MessageSender.SendToTransmitBuffer(data);
                }
                catch(InvalidOperationException)
                { }
            }
        }

        [Fact]
        public void SendToTransmitBuffer_Writes_Message_And_Flushes_Transmitting_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmit, reader => actual = reader.Read().ToArray());

            MessageSender.SendToTransmitPipe(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RegisterSerializationHandler_Serializes_Message_Sent_To_Transmit_Pipe()
        {
            var expected = Guid.NewGuid();
            var actual = default(Guid?);

            HandlersRegistry.RegisterSerializationHandler<Guid>((writer, message) => writer.Write(message.ToByteArray()));
            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmit, reader => actual = new Guid(reader.Read().ToArray()));

            MessageSender.SendToTransmitPipe(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}