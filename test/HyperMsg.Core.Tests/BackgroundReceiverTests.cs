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
        private readonly IPublisher publisher;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public BackgroundReceiverTests()
        {
            messageReceiver = A.Fake<IReceiver<Guid>>();
            publisher = A.Fake<IPublisher>();
            backgroundReceiver = new BackgroundReceiver<Guid>(messageReceiver, publisher);
        }

        [Fact]
        public void Invokes_Message_Handler_After_Switching_To_Reactive_Mode()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;            
            var @event = new ManualResetEventSlim();
            A.CallTo(() => messageReceiver.ReceiveAsync(A<CancellationToken>._)).Returns(Task.FromResult(expected));
            var handler = new DelegateHandler<Guid>(m =>
            {
                actual = m;
                @event.Set();
            });            
            
            backgroundReceiver.Handle(ReceiveMode.SetReactive);            
            @event.Wait(waitTimeout);

            A.CallTo(() => publisher.PublishAsync(expected, A<CancellationToken>._)).MustHaveHappened();
        }

        public void Dispose() => backgroundReceiver.Dispose();
    }
}
