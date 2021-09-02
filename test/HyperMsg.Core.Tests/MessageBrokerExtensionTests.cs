using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new();
        private readonly Guid topicId = Guid.NewGuid();
        private readonly Guid message = Guid.NewGuid();

        
    }
}