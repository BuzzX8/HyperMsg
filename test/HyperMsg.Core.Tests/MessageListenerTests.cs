using System;
using System.Buffers;
using System.Reactive;
using System.Text;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessageListenerTests
    {
        [Fact]
        public void OnNext_Calls_OnNext_Of_Message_Observer_When_Message_Deserialized()
        {
            var expectedMessage = Guid.NewGuid().ToString();
            var actualMessage = (string)null;
            var @event = new ManualResetEventSlim();
            var observer = Observer.Create<string>(s =>
            {
                actualMessage = s;
                @event.Set();
            });
            var listener = new MessageListener<string>(DeserializeString, observer);            

            listener.Start();            
            listener.OnNext(Encoding.UTF8.GetBytes(expectedMessage));
            @event.Wait();

            Assert.Equal(expectedMessage, actualMessage);
        }

        private (string, int) DeserializeString(ReadOnlySequence<byte> buffer)
        {
            var bytes = buffer.First.ToArray();
            return (Encoding.UTF8.GetString(bytes), bytes.Length);
        }
    }
}
