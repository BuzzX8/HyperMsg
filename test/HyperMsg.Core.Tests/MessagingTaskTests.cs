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
        public void SetException_()
        {
            var expectedException = new ArgumentNullException();

            messagingTask.InvokeSetException(expectedException);

            Assert.True(messagingTask.Completion.IsFaulted);
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
