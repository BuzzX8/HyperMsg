using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBufferTests
    {        
        private readonly IBufferWriter<byte> bufferWriter;
        private readonly SerializeAction<Guid> serializeAction;
        private readonly MessageBuffer<Guid> messageBuffer;
        private readonly FlushHandler flushDelegate;

        public MessageBufferTests()
        {            
            bufferWriter = A.Fake<IBufferWriter<byte>>();
            serializeAction = A.Fake<SerializeAction<Guid>>();
            flushDelegate = A.Fake<FlushHandler>();
            messageBuffer = new MessageBuffer<Guid>(bufferWriter, serializeAction, flushDelegate);
        }

        [Fact]
        public async Task FlushAsync_Calls_FlushDelegate()
        {
            var token = new CancellationToken();
            await messageBuffer.FlushAsync(token);
                        
            A.CallTo(() => flushDelegate.Invoke(token)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Serializes_And_Sends_Message()
        {
            var message = Guid.NewGuid();

            await messageBuffer.SendAsync(message, CancellationToken.None);
                        
            A.CallTo(() => serializeAction.Invoke(bufferWriter, message)).MustHaveHappened();
        }

        [Fact]
        public void Write_Calls_Serialize_Action_For_Message()
        {
            var message = Guid.NewGuid();

            messageBuffer.Write(message);

            A.CallTo(() => serializeAction.Invoke(bufferWriter, message)).MustHaveHappened();
        }
    }
}
