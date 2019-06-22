using System;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessageWaiterTests
    {
        private readonly MessageWaiter<string> messageWaiter;
        private readonly string message;

        public MessageWaiterTests()
        {
            messageWaiter = new MessageWaiter<string>();
            message = Guid.NewGuid().ToString();
        }

        [Fact]
        public void WaitAsync_Returns_Completed_Task_If_Message_Has_Been_Set()
        {
            messageWaiter.SetMessage(message);
            var task = messageWaiter.WaitAsync(CancellationToken.None);

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(message, task.Result);
        }

        [Fact]
        public void WaitAsync_Returns_Pending_Task_After_Seconf_Call()
        {
            messageWaiter.SetMessage(message);
            messageWaiter.WaitAsync(CancellationToken.None);
            var task = messageWaiter.WaitAsync(CancellationToken.None);

            Assert.False(task.IsCompletedSuccessfully);
        }

        [Fact]
        public void WaitAsync_Returns_Enqueued_Messages_In_Correct_Order()
        {
            var message2 = Guid.NewGuid().ToString();
            messageWaiter.SetMessage(message);
            messageWaiter.SetMessage(message2);

            messageWaiter.WaitAsync(CancellationToken.None);
            var task2 = messageWaiter.WaitAsync(CancellationToken.None);

            Assert.True(task2.IsCompletedSuccessfully);
            Assert.Equal(message2, task2.Result);
        }

        [Fact]
        public void Cancels_Wait_Task_When_Provided_Token_Signals_Cancellation()
        {
            var tokenSource = new CancellationTokenSource();
            var task = messageWaiter.WaitAsync(tokenSource.Token);

            tokenSource.Cancel();

            Assert.True(task.IsCanceled);
        }

        [Fact]
        public void SetMessage_Completes_Waiting_Task()
        {
            var task = messageWaiter.WaitAsync(CancellationToken.None);
            messageWaiter.SetMessage(message);

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(message, task.Result);
        }

        [Fact]
        public void SetMessage_Throws_Exception_If_Message_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => messageWaiter.SetMessage(null));
        }
    }
}
