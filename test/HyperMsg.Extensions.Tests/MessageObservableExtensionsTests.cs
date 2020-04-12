﻿using FakeItEasy;
using System;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessageObservableExtensionsTests
    {
        private readonly IMessageObservable observable = A.Fake<IMessageObservable>();
        private readonly AsyncAction<Guid> asyncObserver = A.Fake<AsyncAction<Guid>>();
        private readonly Action<Guid> observer = A.Fake<Action<Guid>>();
        private readonly Guid message = Guid.NewGuid();

        [Fact]
        public void SubscribeTransmitter_Subscribes_Observer_For_Transmission()
        {            
            var actual = default(Action<Transmit<Guid>>);
            
            A.CallTo(() => observable.Subscribe(A<Action<Transmit<Guid>>>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<Action<Transmit<Guid>>>(0);
            });

            observable.SubscribeTransmitter(observer);
            Assert.NotNull(actual);
            
            actual.Invoke(new Transmit<Guid>(message));

            A.CallTo(() => observer.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public void SubscribeTransmitter_Subscribes_Async_Observer_For_Transmission()
        {
            var actual = default(AsyncAction<Transmit<Guid>>);

            A.CallTo(() => observable.Subscribe(A<AsyncAction<Transmit<Guid>>>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<AsyncAction<Transmit<Guid>>>(0);
            });

            observable.SubscribeTransmitter(asyncObserver);
            Assert.NotNull(actual);
            
            actual.Invoke(new Transmit<Guid>(message), default);

            A.CallTo(() => asyncObserver.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SubscribeReceiver_Subscribes_Observer_For_Receiving()
        {
            var actual = default(Action<Received<Guid>>);

            A.CallTo(() => observable.Subscribe(A<Action<Received<Guid>>>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<Action<Received<Guid>>>(0);
            });

            observable.SubscribeReceiver(observer);
            Assert.NotNull(actual);

            actual.Invoke(new Received<Guid>(message));

            A.CallTo(() => observer.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public void SubscribeReceiver_Subscribes_Async_Observer_For_Receiving()
        {
            var actual = default(AsyncAction<Received<Guid>>);

            A.CallTo(() => observable.Subscribe(A<AsyncAction<Received<Guid>>>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<AsyncAction<Received<Guid>>>(0);
            });

            observable.SubscribeReceiver(asyncObserver);
            Assert.NotNull(actual);

            actual.Invoke(new Received<Guid>(message), default);

            A.CallTo(() => asyncObserver.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}