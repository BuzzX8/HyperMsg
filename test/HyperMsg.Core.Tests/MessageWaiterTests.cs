using System;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessageWaiterTests
    {
        [Fact]
        public void WaitAsync_Returns_Completed_Task_If_Message_Has_Been_Set()
        {
            var message = Guid.NewGuid();
            var waiter = new MessageWaiter<Guid>();

            waiter.SetMessage(message);
            var task = waiter.WaitAsync(CancellationToken.None);

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(message, task.Result);
        }

        [Fact]
        public void SetMessage_Completes_Waiting_Task()
        {
            var message = Guid.NewGuid();
            var waiter = new MessageWaiter<Guid>();

            var task = waiter.WaitAsync(CancellationToken.None);
            waiter.SetMessage(message);

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(message, task.Result);
        }
    }
}
