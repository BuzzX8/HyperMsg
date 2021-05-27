using FakeItEasy;
using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg
{
    public class MessagingTaskTests
    {
        private readonly IMessagingContext messagingContext;
        private readonly IList<IDisposable> autoDisposables;
        private readonly MessagingTaskMock messagingTask;

        public MessagingTaskTests()
        {
            messagingContext = A.Fake<IMessagingContext>();
            messagingTask = new MessagingTaskMock(messagingContext);
        }

        [Fact]
        public void SetCompleted_Completes_Completion_Task()
        {
            var completionTask = messagingTask.Completion;
            Assert.False(completionTask.IsCompleted);

            messagingTask.InvokeSetCompleted();

            
            Assert.True(completionTask.IsCompleted);
            Assert.False(completionTask.IsFaulted);
        }

        [Fact]
        public void SetException_Sets_Completion_Task_As_Fault()
        {
            var expectedException = new ArgumentNullException();
            var completionTask = messagingTask.Completion;

            messagingTask.InvokeSetException(expectedException);

            Assert.True(completionTask.IsFaulted);
            var actualException = completionTask.Exception.InnerException;
            Assert.Same(expectedException, actualException);
        }
    }

    public class MessagingTaskMock : MessagingTask
    {
        public MessagingTaskMock(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        public void InvokeSetCompleted() => SetCompleted();

        public void InvokeSetException(Exception exception) => SetException(exception);
    }
}
