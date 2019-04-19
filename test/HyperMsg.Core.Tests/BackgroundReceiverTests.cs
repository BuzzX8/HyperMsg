using FakeItEasy;
using System;

namespace HyperMsg.Transciever
{
    public class BackgroundReceiverTests
    {
        private readonly IReceiver<Guid> messageReceiver;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        public BackgroundReceiverTests()
        {
            messageReceiver = A.Fake<IReceiver<Guid>>();
            backgroundReceiver = new BackgroundReceiver<Guid>(messageReceiver);
        }
    }
}
