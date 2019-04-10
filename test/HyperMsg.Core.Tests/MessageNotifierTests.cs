using FakeItEasy;
using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class MessageNotifierTests
    {
        [Fact]
        public void ReadBuffer_Does_Not_Calls_Handler_If_No_Bytes_Readen()
        {
            var reader = new MessageNotifier<Guid>(b => new DeserializationResult<Guid>(0, Guid.Empty));
            var handler = A.Fake<Action<Guid>>();
            reader.Received += handler;

            reader.ReadBuffer(default);

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ReadBuffer_Calls_Handler_For_Deserialized_Message()
        {
            var byteCount = 10;
            var message = Guid.NewGuid();
            
            var reader = new MessageNotifier<Guid>(b => new DeserializationResult<Guid>(byteCount, message));
            var handler = A.Fake<Action<Guid>>();
            reader.Received += handler;

            var readed = reader.ReadBuffer(new ReadOnlySequence<byte>());
            
            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
            Assert.Equal(byteCount, readed);
        }
    }
}
