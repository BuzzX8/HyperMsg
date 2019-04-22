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
            var isTaskCompleted = false;

            backgroundReceiver.BackgroundTaskCompleted += t =>
            {
                isTaskCompleted = true;
                @event.Set();
            };
            backgroundReceiver.Run();
            
            backgroundReceiver.Dispose();
            @event.Wait(waitTimeout);

            Assert.True(isTaskCompleted);
        }

        [Fact]
        public void UnhandledException_Rises_With_Correct_Exception()
        {
            var wasInvoked = false;            
            var @event = new ManualResetEventSlim();

            backgroundReceiver.UnhandledException += e =>
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
            backgroundReceiver.Stop();
        }
    }
}
