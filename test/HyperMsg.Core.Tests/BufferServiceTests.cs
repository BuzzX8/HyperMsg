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
        public void SendToBuffer_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Transmitting, bufferReader);
            MessageSender.SendToBuffer(PipeType.Transmitting, Guid.NewGuid().ToByteArray(), true);

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToBuffer_Does_Not_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterPipeHandler(PipeType.Transmitting, bufferReader);
            MessageSender.SendToBuffer(PipeType.Transmitting, Guid.NewGuid().ToByteArray(), false);

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendToBuffer_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Receiving, reader => actual = reader.Read().ToArray());

            MessageSender.SendToBuffer(PipeType.Receiving, new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendFlushBufferCommand_Invokes_Handler_Registered_By_RegisterBufferFlushSegmentReader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmitting, reader => actual = reader.Read().ToArray());
            MessageSender.SendToBuffer(PipeType.Transmitting, expected);

            MessageSender.SendFlushBufferCommand(PipeType.Transmitting);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendFlushBufferCommand_Invokes_Async_Handler_Registered_By_RegisterBufferFlushSegmentReader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmitting, (buffer, _) =>
            {
                actual = buffer.Read().ToArray();
                return Task.CompletedTask;
            });
            MessageSender.SendToBuffer(PipeType.Transmitting, expected);

            MessageSender.SendFlushBufferCommand(PipeType.Transmitting);

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

            HandlersRegistry.RegisterPipeHandler<IBufferReader>(PipeType.Transmitting, reader => actual = reader.Read().ToArray());

            MessageSender.SendToTransmitBuffer(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}