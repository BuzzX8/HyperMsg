using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Extensions
{
    public class TaskExtensionsTests
    {
        [Fact]
        public async Task OnSuccessfullyComplete_Invokes_CompleteHandler_For_Completed_Task()
        {
            var completeHandler = A.Fake<Action>();

            await Task.CompletedTask.OnSuccessfullyComplete(completeHandler);

            A.CallTo(() => completeHandler.Invoke()).MustHaveHappened();
        }

        [Fact]
        public async Task OnSuccessfullyComplete_Does_Not_Invokes_CompleteHandler_For_Faulted_Task()
        {
            var completeHandler = A.Fake<Action>();

            await Assert.ThrowsAsync<TaskCanceledException>(() =>Task.FromException(new Exception()).OnSuccessfullyComplete(completeHandler));

            A.CallTo(() => completeHandler.Invoke()).MustNotHaveHappened();
        }

        [Fact]
        public async Task OnSuccessfullyComplete_Does_Not_Invokes_CompleteHandler_For_Cancelled_Task()
        {
            var tokenSource = new CancellationTokenSource();
            var completeHandler = A.Fake<Action>();

            await Assert.ThrowsAsync<TaskCanceledException>(() => Task.FromCanceled(new(true)).OnSuccessfullyComplete(completeHandler));

            A.CallTo(() => completeHandler.Invoke()).MustNotHaveHappened();
        }

        [Fact]
        public async Task OnSuccessfullyComplete_Invokes_CompleteHandler_For_Completed_Task_With_Result()
        {
            var completeHandler = A.Fake<Action<Guid>>();
            var result = Guid.NewGuid();

            await Task.FromResult(result).OnSuccessfullyComplete(completeHandler);

            A.CallTo(() => completeHandler.Invoke(result)).MustHaveHappened();
        }
    }
}
