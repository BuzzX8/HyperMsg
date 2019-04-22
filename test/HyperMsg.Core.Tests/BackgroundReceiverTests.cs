using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundReceiverTests : IDisposable
    {
        private readonly IReceiver<Guid> messageReceiver;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public BackgroundReceiverTests()
        {
            messageReceiver = A.Fake<IReceiver<Guid>>();
            backgroundReceiver = new BackgroundReceiver<Guid>(messageReceiver);            
        }

        [Fact]
        public void Run_Invokes_MessageReceived()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;            
            var @event = new ManualResetEventSlim();
            A.CallTo(() => messageReceiver.ReceiveAsync(A<CancellationToken>._)).Returns(Task.FromResult(expected));

            Assert.False(backgroundReceiver.IsRunning);
            backgroundReceiver.MessageReceived += m =>
            {
                actual = m;
                @event.Set();
            };
            backgroundReceiver.Run();
            Assert.True(backgroundReceiver.IsRunning);
            @event.Wait(waitTimeout);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_Initiates_Background_Task_Cancellation()
        {
            var @event = new ManualResetEventSlim();
            var @event2 = new ManualResetEventSlim();
            var wasInvoked = false;

            backgroundReceiver.MessageReceived += m =>
            {
                @event.Wait();
                wasInvoked = true;
                event2.Set();
            };
            backgroundReceiver.Run();
            
            backgroundReceiver.Dispose();
            @event.Set();
            event2.Wait(waitTimeout);

            Assert.False(wasInvoked);
            Assert.False(backgroundReceiver.IsRunning);
        }

        [Fact]
        public void OnUnhandledException_Rises_With_Correct_Exception()
        {
            var wasInvoked = false;            
            var @event = new ManualResetEventSlim();

            backgroundReceiver.OnUnhandlerException += e =>
            {
                wasInvoked = true;
                @event.Set();
            };
            backgroundReceiver.MessageReceived += m => throw new Exception();

            backgroundReceiver.Run();
            @event.Wait(waitTimeout);

            Assert.True(wasInvoked);
        }

        public void Dispose()
        {
            backgroundReceiver.Dispose();
        }
    }
}
