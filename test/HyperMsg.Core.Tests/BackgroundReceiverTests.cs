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
        private readonly Func<IEnumerable<IHandler<Guid>>> handlerProvider;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public BackgroundReceiverTests()
        {
            messageReceiver = A.Fake<IReceiver<Guid>>();
            handlerProvider = A.Fake<Func<IEnumerable<IHandler<Guid>>>>();
            backgroundReceiver = new BackgroundReceiver<Guid>(messageReceiver, handlerProvider);            
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
            A.CallTo(() => handlerProvider.Invoke()).Returns(new[] { handler });
            
            backgroundReceiver.Handle(ReceiveMode.Reactive);            
            @event.Wait(waitTimeout);

            Assert.Equal(expected, actual);
        }

        public void Dispose() => backgroundReceiver.Dispose();
    }
}
