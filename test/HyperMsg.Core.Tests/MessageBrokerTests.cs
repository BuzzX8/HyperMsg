﻿using FakeItEasy;
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
            broker.AddObserver(observer);

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
                broker.AddObserver(observer);
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
                broker.AddObserver(observer);
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
            broker.AddObserver(observer);

            await broker.SendAsync(message, tokenSource.Token);

            A.CallTo(() => observer.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Throw_Exception_When_New_Observer_Subscribed()
        {
            broker.AddObserver<Guid>(m =>
            {
                broker.AddObserver<string>(s => { });
            });

            broker.Send(Guid.NewGuid());
        }

        [Fact]
        public void Send_Does_Not_Invokes_Unsubscribed_Observers()
        {
            var observer = A.Fake<Action<Guid>>();
            var subscription = broker.AddObserver(observer);

            subscription.Dispose();
            broker.Send(Guid.NewGuid());

            A.CallTo(() => observer.Invoke(A<Guid>._)).MustNotHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Invokes_Unsubscribed_Async_Observers()
        {
            var observer = A.Fake<AsyncAction<Guid>>();
            var subscription = broker.AddObserver(observer);

            subscription.Dispose();
            broker.Send(Guid.NewGuid());

            A.CallTo(() => observer.Invoke(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public void Send_Does_Not_Throw_Exception_If_Cancelling_Subscription_Inside_Observer()
        {
            var subscription = broker.AddObserver<Guid>(m => { });
            broker.AddObserver<Guid>(m => subscription.Dispose());

            broker.Send(Guid.NewGuid());
        }

        [Fact]
        public void Send_Invokes_Handler_If_Type_Compatible()
        {
            var message = Guid.NewGuid();
            var actualMessage = Guid.Empty;

            broker.AddObserver<Guid>(m => actualMessage = m);

            broker.Send<object>(message);

            Assert.Equal(message, actualMessage);
        }

        [Fact]
        public async Task SendAsync_Invokes_Handler_If_Type_Compatible()
        {
            var message = Guid.NewGuid();
            var actualMessage = Guid.Empty;

            broker.AddObserver<Guid>(m => actualMessage = m);

            await broker.SendAsync<object>(message, tokenSource.Token);

            Assert.Equal(message, actualMessage);
        }
    }
}
