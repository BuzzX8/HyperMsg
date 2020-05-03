using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class TaskExecutorTests
    {
        private readonly TaskExecutor taskExecutor;
        private readonly ITask task;
        private readonly ITask<Guid> resultTask;

        private ITaskCompletionSource completionSource;
        private ITaskCompletionSource<Guid> resultCompletionSource;

        public TaskExecutorTests()
        {
            taskExecutor = new TaskExecutor();
            task = A.Fake<ITask>();
            resultTask = A.Fake<ITask<Guid>>();

            A.CallTo(() => task.InitializeAsync(A<ITaskCompletionSource>._, A<CancellationToken>._)).Invokes(foc =>
            {
                completionSource = foc.GetArgument<ITaskCompletionSource>(0);
            });
            A.CallTo(() => resultTask.InitializeAsync(A<ITaskCompletionSource<Guid>>._, A<CancellationToken>._)).Invokes(foc =>
            {
                resultCompletionSource = foc.GetArgument<ITaskCompletionSource<Guid>>(0);
            });
        }

        [Fact]
        public void ExecuteAsync_Invokes_InitializeAsync()
        {
            _ = taskExecutor.ExecuteAsync(task, default);

            A.CallTo(() => task.InitializeAsync(A<ITaskCompletionSource>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void ExecuteAsync_Provides_Not_Null_CompletionSource()
        {
            var t = taskExecutor.ExecuteAsync(task, default);

            Assert.False(t.IsCompleted);
            Assert.NotNull(completionSource);
        }

        [Fact]
        public void ExecuteAsync_Completes_Task_When_SetCompleted_Invoked()
        {
            var t = taskExecutor.ExecuteAsync(task, default);

            completionSource.SetCompleted();

            Assert.True(t.IsCompleted);
            A.CallTo(() => task.Dispose()).MustHaveHappened();
        }

        [Fact]
        public void ExecuteAsync_Completes_Result_Task_When_SetResult_Invoked()
        {
            var t = taskExecutor.ExecuteAsync(resultTask, default);
            var result = Guid.NewGuid();

            resultCompletionSource.SetResult(result);

            Assert.True(t.IsCompleted);
            Assert.Equal(result, t.Result);
            //A.CallTo(() => task.Dispose()).MustHaveHappened();
        }

        [Fact]
        public void ExecuteAsync_()
        {
            var tokenSource = new CancellationTokenSource();

            var t = taskExecutor.ExecuteAsync(task, tokenSource.Token);
            tokenSource.Cancel();

            //Assert.True(t.IsCanceled);
            A.CallTo(() => task.Dispose()).MustHaveHappened();
        }
    }
}
