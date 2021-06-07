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
        public void Send_Invokes_Single_Observers()
        {
            var observer = A.Fake<AsyncAction<Guid>>();
            var message = Guid.NewGuid();
            broker.RegisterHandler(observer);

            broker.Send(message);

            A.CallTo(() => observer.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void Send_Invokes_Multiple_Observers()
        {
            var observers = A.CollectionOfFake<Action<Guid>>(3);
            var message = Guid.NewGuid();

            foreach (var observer in observers)
            {
                broker.RegisterHandler(observer);
            }

            broker.Send(message);

            foreach (var observer in observers)
            {
                A.CallTo(() => observer.Invoke(message)).MustHaveHappened();
            }
        }

        [Fact]
        public void Send_Invokes_Multiple_Async_Observers()
        {
            var observers = A.CollectionOfFake<AsyncAction<Guid>>(3);
            var message = Guid.NewGuid();
            
            foreach(var observer in observers)
            {
                broker.RegisterHandler(observer);
            }

            broker.Send(message);

            foreach (var observer in observers)
            {
                A.CallTo(() => observer.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
            }
        }

        [Fact]
        public async Task SendAsync_Invokes_Async_Observers()
        {
            var observer = A.Fake<AsyncAction<Guid>>();
            var message = Guid.NewGuid();
            broker.RegisterHandler(observer);

            await broker.SendAsync(message, tokenSource.Token);

            A.CallTo(() => observer.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Throw_Exception_When_New_Observer_Subscribed()
        {
            broker.RegisterHandler<Guid>(m =>
            {
                broker.RegisterHandler<string>(s => { });
            });

            broker.Send(Guid.NewGuid());
        }

        [Fact]
        public void Send_Does_Not_Invokes_Unsubscribed_Observers()
        {
            var observer = A.Fake<Action<Guid>>();
            var subscription = broker.RegisterHandler(observer);

            subscription.Dispose();
            broker.Send(Guid.NewGuid());

            A.CallTo(() => observer.Invoke(A<Guid>._)).MustNotHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Invokes_Unsubscribed_Async_Observers()
        {
            var observer = A.Fake<AsyncAction<Guid>>();
            var subscription = broker.RegisterHandler(observer);

            subscription.Dispose();
            broker.Send(Guid.NewGuid());

            A.CallTo(() => observer.Invoke(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Throw_Exception_If_Cancelling_Subscription_Inside_Observer()
        {
            var subscription = broker.RegisterHandler<Guid>(m => { });
            broker.RegisterHandler<Guid>(m => subscription.Dispose());

            broker.Send(Guid.NewGuid());
        }

        [Fact]
        public void SendWaitForMessageRequest_Result()
        {
            var message = Guid.NewGuid();

            var task = broker.SendWaitForMessageRequest<Guid>(m => m == message, default);
            broker.Send(message);

            Assert.True(task.IsCompleted);
            Assert.Equal(message, task.Result);
        }

        [Fact]
        public void SendWaitForMessageRequest_Exception()
        {
            var message = Guid.NewGuid();
            var exception = new InvalidCastException();

            var task = broker.SendWaitForMessageRequest<Guid>(m => throw exception, default);
            broker.Send(Guid.NewGuid());

            var _ = Assert.Throws<AggregateException>(() => task.Wait(1000));            
        }

        [Fact]
        public void SendWaitForMessageRequest_Cancel()
        {
            var cancellation = new CancellationTokenSource();

            var task = broker.SendWaitForMessageRequest<Guid>(m => false, cancellation.Token);
            broker.Send(Guid.NewGuid());

            cancellation.Cancel();

            Assert.True(task.IsCanceled);
        }
    }
}
