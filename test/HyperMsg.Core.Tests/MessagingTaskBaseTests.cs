using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessagingTaskBaseTests
    {
        private readonly IMessagingContext messagingContext;
        private readonly IList<IDisposable> autoDisposables;
        private readonly MessagingTaskBaseMock messagingTask;

        public MessagingTaskBaseTests()
        {
            messagingContext = A.Fake<IMessagingContext>();
            autoDisposables = A.CollectionOfFake<IDisposable>(10);
            messagingTask = new MessagingTaskBaseMock(autoDisposables, messagingContext);
        }

        [Fact]
        public void Dispose_Disposes_Auto_Disposables_If_Start_Invoked()
        {
            messagingTask.InvokeStart();
            messagingTask.Dispose();

            foreach (var disposable in autoDisposables)
            {
                A.CallTo(() => disposable.Dispose()).MustHaveHappened();
            }
        }

        [Fact]
        public void Dispose_Does_Not_Disposes_Auto_Disposables_If_Start_Does_Not_Invoked()
        {
            messagingTask.Dispose();

            foreach (var disposable in autoDisposables)
            {
                A.CallTo(() => disposable.Dispose()).MustNotHaveHappened();
            }
        }

        [Fact]
        public void Start_Invokes_Begin()
        {
            var beginFunc = A.Fake<Func<Task>>();
            messagingTask.BeginFunc = beginFunc;

            messagingTask.InvokeStart();

            A.CallTo(() => beginFunc.Invoke()).MustHaveHappened();
        }

        [Fact]
        public void Start_Invokes_SetException_If_Task_Fails()
        {
            var beginFunc = A.Fake<Func<Task>>();
            var waitEvent = new ManualResetEventSlim();
            var expectedException = new ArgumentOutOfRangeException();
            A.CallTo(() => beginFunc.Invoke()).Invokes(foc => waitEvent.Set()).Returns(Task.FromException(expectedException));
            messagingTask.BeginFunc = beginFunc;

            messagingTask.InvokeStart();

            //waitEvent.Wait(TimeSpan.FromSeconds(1));
            Assert.NotNull(messagingTask.Exception);
        }
    }

    public class MessagingTaskBaseMock : MessagingTaskBase
    {
        private readonly IList<IDisposable> autoDisposables;

        public MessagingTaskBaseMock(IList<IDisposable> autoDisposables, IMessagingContext messagingContext) : base(messagingContext) => this.autoDisposables = autoDisposables;

        public Func<Task> BeginFunc { get; set; }

        public Exception Exception { get; set; }

        public void InvokeStart() => Start();

        protected override IEnumerable<IDisposable> GetAutoDisposables() => autoDisposables;

        protected override Task BeginAsync() => BeginFunc?.Invoke() ?? base.BeginAsync();

        protected override void SetException(Exception exception) => Exception = exception;
    }
}
