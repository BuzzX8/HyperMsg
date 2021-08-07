using FakeItEasy;
using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : HostFixture
    {
        [Fact]
        public void SendToTransmitBuffer_Invokes_BufferReader_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterTransmitBufferHandler(bufferReader);
            MessageSender.SendToTransmitBuffer(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTransmitBufferAsync_Invokes_BufferReader_Handler()
        {
            var bufferReader = A.Fake<AsyncAction<IBufferReader>>();

            HandlersRegistry.RegisterTransmitBufferHandler(bufferReader);
            await MessageSender.SendToTransmitBufferAsync(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToTransmitBuffer_Does_Not_Invokes_BufferReader_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterTransmitBufferHandler(bufferReader);
            MessageSender.SendToTransmitBuffer(Guid.NewGuid().ToByteArray(), false);

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendToReceiveBuffer_Invokes_BufferReader_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterReceiveBufferHandler(bufferReader);
            MessageSender.SendToReceiveBuffer(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToReceiveBufferAsync_Invokes_BufferReader_Handler()
        {
            var bufferReader = A.Fake<AsyncAction<IBufferReader>>();

            HandlersRegistry.RegisterReceiveBufferHandler(bufferReader);
            await MessageSender.SendToReceiveBufferAsync(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToReceiveBuffer_Does_Not_Invokes_BufferReader_Handler()
        {
            var bufferReader = A.Fake<Action<IBufferReader>>();

            HandlersRegistry.RegisterReceiveBufferHandler(bufferReader);
            MessageSender.SendToReceiveBuffer(Guid.NewGuid().ToByteArray(), false);

            A.CallTo(() => bufferReader.Invoke(A<IBufferReader>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendTransmitToBuffer_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = reader.Read().ToArray());
            MessageSender.SendToTransmitBuffer(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SendTransmitToBufferAsync_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = reader.Read().ToArray());
            await MessageSender.SendToTransmitBufferAsync(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendReceiveToBuffer_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());
            MessageSender.SendToReceiveBuffer(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SendReceiveToBufferAsync_Writes_Stream_Content_To_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());
            await MessageSender.SendToReceiveBufferAsync(new MemoryStream(expected));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendToReceiveBuffer_Invokes_Write_Delegate_For_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            void WriteAction(IBufferWriter writer) => writer.Write(expected);

            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());
            MessageSender.SendToReceiveBuffer(WriteAction);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SendToReceiveBufferAsync_Invokes_Write_Delegate_For_Buffer()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            void WriteAction(IBufferWriter writer) => writer.Write(expected);

            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());
            await MessageSender.SendToReceiveBufferAsync(WriteAction);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SendToReceiveBuffer_Invokes_Write_Delegate_For_Buffer_Adapter()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            void WriteAction(IBufferWriter<byte> writer) => writer.Write(expected);

            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());
            MessageSender.SendToReceiveBuffer(WriteAction);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SendToReceiveBufferAsync_Invokes_Write_Delegate_For_Buffer_Adapter()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);

            void WriteAction(IBufferWriter<byte> writer) => writer.Write(expected);

            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());
            await MessageSender.SendToReceiveBufferAsync(WriteAction);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RegisterBufferFlushDataHandler_Advances_Buffer_Reader()
        {
            var data = Guid.NewGuid().ToByteArray();
            var bufferCapacity = GetRequiredService<IBufferContext>().TransmittingBuffer.Writer.GetMemory().Length;
            var iterationCount = (bufferCapacity / data.Length) * 10;
            HandlersRegistry.RegisterTransmitBufferHandler(data => { });

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

            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = reader.Read().ToArray());
            MessageSender.SendToTransmitTopic(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RegisterSerializationHandler_Serializes_Message_Sent_To_Transmit_Topic()
        {
            var expected = Guid.NewGuid();
            var actual = default(Guid?);

            static void Serialize(IBufferWriter writer, Guid message) => writer.Write(message.ToByteArray());

            HandlersRegistry.RegisterSerializationHandler<Guid>(Serialize);
            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = new Guid(reader.Read().ToArray()));
            MessageSender.SendToTransmitTopic(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RegisterSerializationHandler_Serializes_Message_Sent_To_Byte_Transmit_Topic()
        {
            var expected = Guid.NewGuid();
            var actual = default(Guid?);

            static void Serialize(IBufferWriter<byte> writer, Guid message) => writer.Write(message.ToByteArray());

            HandlersRegistry.RegisterSerializationHandler<Guid>(Serialize);
            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = new Guid(reader.Read().ToArray()));
            MessageSender.SendToTransmitTopic(expected);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}