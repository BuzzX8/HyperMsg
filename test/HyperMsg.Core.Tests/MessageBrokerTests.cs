using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerTests
    {
        private readonly MessageBroker broker = new MessageBroker();
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        [Fact]
        public void Send_Invokes_Async_Observers()
        {
            var observer = A.Fake<AsyncAction<Guid>>();
            var message = Guid.NewGuid();
            broker.Subscribe(observer);

            broker.Send(message);

            A.CallTo(() => observer.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Invokes_Async_Observers()
        {
            var observer = A.Fake<AsyncAction<Guid>>();
            var message = Guid.NewGuid();
            broker.Subscribe(observer);

            await broker.SendAsync(message, tokenSource.Token);

            A.CallTo(() => observer.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Throw_Exception_When_New_Observer_Subscribed()
        {
            broker.Subscribe<Guid>(m =>
            {
                broker.Subscribe<string>(s => { });
            });

            broker.Send(Guid.NewGuid());
        }
    }
}
