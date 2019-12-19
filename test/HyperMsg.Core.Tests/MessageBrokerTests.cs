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
        public void Send_Invokes_Handler()
        {
            var handler = A.Fake<Action<Guid>>();
            var message = Guid.NewGuid();
            broker.Register(handler);

            broker.Send(message);

            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Invokes_Handler()
        {
            var handler = A.Fake<Action<Guid>>();
            var message = Guid.NewGuid();
            broker.Register(handler);

            await broker.SendAsync(message, tokenSource.Token);

            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public void Send_Invokes_Async_Handlers()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            var message = Guid.NewGuid();
            broker.Register(handler);

            broker.Send(message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Invokes_Async_Handlers()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            var message = Guid.NewGuid();
            broker.Register(handler);

            await broker.SendAsync(message, tokenSource.Token);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Throw_Exception_When_New_Handler_Registred()
        {
            broker.Register<Guid>(m =>
            {
                broker.Register<string>(s => { });
            });

            broker.Send(Guid.NewGuid());
        }
    }
}
