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
        public void Run_Invokes_Message_Handler()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;            
            var @event = new ManualResetEventSlim();
            A.CallTo(() => messageReceiver.ReceiveAsync(A<CancellationToken>._)).Returns(Task.FromResult(expected));
                        
            backgroundReceiver.Run(m =>
            {
                actual = m;
                @event.Set();
            });            
            @event.Wait(waitTimeout);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_Stops_Background_Task()
        {
            var @event = new ManualResetEventSlim();
            var @event2 = new ManualResetEventSlim();
            var wasInvoked = false;

            var disp = backgroundReceiver.Run(m =>
            {
                @event.Wait();
                wasInvoked = true;
                event2.Set();
            });
            Assert.Same(backgroundReceiver, disp);

            backgroundReceiver.Dispose();
            @event.Set();
            event2.Wait(waitTimeout);

            Assert.False(wasInvoked);
        }

        [Fact(Skip = "TODO: ")]
        public void OnUnhandledException_Rises_With_Correct_Exception()
        {
            var expected = new ArgumentNullException();
            var actual = (Exception)null;
            backgroundReceiver.OnUnhandlerException += e => actual = e;
            var @event = new ManualResetEventSlim();

            backgroundReceiver.Run(m =>
            {
                try
                {
                    throw expected;
                }
                finally
                {
                    @event.Set();
                }
            });
            @event.Wait(waitTimeout);

            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            backgroundReceiver.Dispose();
        }
    }
}
