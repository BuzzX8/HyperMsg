using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessagingObjectTests
    {
        private readonly IMessagingContext messagingContext;
        private readonly IList<IDisposable> autoDisposables;
        private readonly MessagingObjectMock messagingObject;

        public MessagingObjectTests()
        {
            messagingContext = A.Fake<IMessagingContext>();
            autoDisposables = A.CollectionOfFake<IDisposable>(10);
            messagingObject = new MessagingObjectMock(autoDisposables, messagingContext);
        }

        [Fact]
        public void RegisterAutoDisposables_Registeres_Disposables_For_Auto_Disposing()
        {
            messagingObject.InvokeRegisterAutoDisposables();
            messagingObject.Dispose();

            foreach(var disposable in autoDisposables)
            {
                A.CallTo(() => disposable.Dispose()).MustHaveHappened();
            }
        }

        [Fact]
        public void RegisterAutoDisposables_Does_Not_Registeres_Disposables_For_Auto_Disposing()
        {
            messagingObject.Dispose();

            foreach (var disposable in autoDisposables)
            {
                A.CallTo(() => disposable.Dispose()).MustNotHaveHappened();
            }
        }

        [Fact]
        public void RegisterHandler_Registers_Handler_With_MessagingContext()
        {
            var handler = A.Fake<Action<string>>();

            messagingObject.RegisterHandler(handler);

            A.CallTo(() => messagingContext.HandlersRegistry.RegisterHandler(handler)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Registers_Async_Handler_With_MessagingContext()
        {
            var handler = A.Fake<AsyncAction<string>>();

            messagingObject.RegisterHandler(handler);

            A.CallTo(() => messagingContext.HandlersRegistry.RegisterHandler(handler)).MustHaveHappened();
        }

        [Fact]
        public void Send_Sends_Message_With_MessagingContext()
        {
            var message = Guid.NewGuid();

            messagingObject.Send(message);

            A.CallTo(() => messagingContext.Sender.Send(message)).MustHaveHappened();
        }

        [Fact]
        public void SendAsync_Sends_Message_With_MessagingContext()
        {
            var message = Guid.NewGuid();
            var token = new CancellationToken();

            messagingObject.SendAsync(message, token);

            A.CallTo(() => messagingContext.Sender.SendAsync(message, token)).MustHaveHappened();
        }
    }

    public class MessagingObjectMock : MessagingObject
    {
        private readonly IList<IDisposable> autoDisposables;

        public MessagingObjectMock(IList<IDisposable> autoDisposables, IMessagingContext messagingContext) : base(messagingContext) => this.autoDisposables = autoDisposables;

        protected override IEnumerable<IDisposable> GetAutoDisposables() => autoDisposables;

        public void InvokeRegisterAutoDisposables() => RegisterAutoDisposables();
    }
}
