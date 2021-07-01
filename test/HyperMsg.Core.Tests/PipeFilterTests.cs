using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class PipeFilterTests
    {
        [Fact]
        public void SendToPipe_Extension_Invokes_Filter_Predicate()
        {
            var messageSender = A.Fake<IMessageSender>();
            var filterFunc = A.Fake<Func<object, object, string, bool>>();

            var pipeId = Guid.NewGuid();
            var portId = Guid.NewGuid();
            var message = Guid.NewGuid().ToString();

            var filter = new PipeFilter<string>(messageSender, filterFunc);

            filter.SendToPipe(pipeId, portId, message);

            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).MustHaveHappened();
        }
    }
}
