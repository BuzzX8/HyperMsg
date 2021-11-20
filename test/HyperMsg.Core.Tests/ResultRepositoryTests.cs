using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class ResultRepositoryTests : HostFixture
    {
        private readonly IResultRepository resultRepository;

        public ResultRepositoryTests()
        {
            resultRepository = GetRequiredService<IResultRepository>();
        }

        [Fact]
        public void GetResultPromise_Returns_Pending_Task_If_No_Result_For_Key()
        {
            var key = Guid.NewGuid();

            var promise = resultRepository.GetResultPromise<string>(key);

            Assert.Equal(TaskStatus.WaitingForActivation, promise.Status);
        }

        [Fact]
        public void GetResultPromise_Returns_Completed_Task_With_Result()
        {
            var key = Guid.NewGuid();
            resultRepository.SetResult(key, key.ToString());

            var promise = resultRepository.GetResultPromise<string>(key);

            Assert.Equal(TaskStatus.RanToCompletion, promise.Status);
            Assert.Equal(key.ToString(), promise.Result);
        }

        [Fact]
        public void SetResult_Completes_Result_Promise()
        {
            var key = Guid.NewGuid();
            var promise = resultRepository.GetResultPromise<string>(key);

            resultRepository.SetResult(key, key.ToString());

            Assert.Equal(TaskStatus.RanToCompletion, promise.Status);
            Assert.Equal(key.ToString(), promise.Result);
        }
    }
}
