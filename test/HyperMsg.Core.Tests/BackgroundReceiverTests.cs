using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundReceiverTests : IDisposable
    {
        private readonly IReceiver<Guid> messageReceiver;
        private readonly ISender sender;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public BackgroundReceiverTests()
        {
            messageReceiver = A.Fake<IReceiver<Guid>>();
            sender = A.Fake<ISender>();
            backgroundReceiver = new BackgroundReceiver<Guid>(messageReceiver, sender);
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
            
            backgroundReceiver.Handle(ReceiveMode.Reactive);            
            @event.Wait(waitTimeout);

            A.CallTo(() => sender.SendAsync(expected, A<CancellationToken>._)).MustHaveHappened();
        }

        public void Dispose() => backgroundReceiver.Dispose();
    }
}
