using System;

namespace HyperMsg.Integration
{
    public abstract class TestFixtureBase : SocketFixtureBase<Guid>
    {
        protected TestFixtureBase(int port) : base(port)
        { }

        protected override void ConfigureSerializer(IConfigurable configurable) => configurable.UseGuidSerializer();
    }
}
