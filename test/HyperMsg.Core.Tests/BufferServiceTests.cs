using FakeItEasy;
using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : ServiceHostFixture
    {
        [Fact]
        public void SendWriteToBufferCommand_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<BufferReader>();

            HandlersRegistry.RegisterBufferFlushReader(BufferType.Transmitting, bufferReader);
            MessageSender.SendToBuffer(BufferType.Transmitting, Guid.NewGuid().ToByteArray(), true);

            A.CallTo(() => bufferReader.Invoke(A<ReadOnlySequence<byte>>._)).MustHaveHappened();
        }

        [Fact]
        public void SendWriteToBufferCommand_Does_Not_Invokes_FlushCommand_Handler()
        {
            var bufferReader = A.Fake<BufferReader>();

            HandlersRegistry.RegisterBufferFlushReader(BufferType.Transmitting, bufferReader);
            MessageSender.SendToBuffer(BufferType.Transmitting, Guid.NewGuid().ToByteArray(), false);

            A.CallTo(() => bufferReader.Invoke(A<ReadOnlySequence<byte>>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendWriteToBufferCommand_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterBufferFlushSegmentReader(BufferType.Receiving, buffer =>
            {
                actual = buffer.ToArray();
                return buffer.Length;
            });

            MessageSender.SendToBuffer(BufferType.Receiving, new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendFlushBufferCommand_Invokes_Handler_Registered_By_RegisterBufferFlushSegmentReader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterBufferFlushSegmentReader(BufferType.Transmitting, buffer =>
            {
                actual = buffer.ToArray();
                return buffer.Length;
            });
            MessageSender.SendToBuffer(BufferType.Transmitting, expected);

            MessageSender.SendFlushBufferCommand(BufferType.Transmitting);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendFlushBufferCommand_Invokes_Async_Handler_Registered_By_RegisterBufferFlushSegmentReader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterBufferFlushReader(BufferType.Transmitting, (buffer, _) =>
            {
                actual = buffer.ToArray();
                return Task.FromResult((int)buffer.Length);
            });
            MessageSender.SendToBuffer(BufferType.Transmitting, expected);

            MessageSender.SendFlushBufferCommand(BufferType.Transmitting);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendTransmitMessageCommand_Writes_Message_And_Flushes_Transmitting_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterBufferFlushSegmentReader(BufferType.Transmitting, buffer =>
            {
                actual = buffer.ToArray();
                return buffer.Length;
            });

            MessageSender.SendToTransmitBuffer(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}