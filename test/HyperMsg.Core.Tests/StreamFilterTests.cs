using System.Net.Security;
using Xunit;

namespace HyperMsg
{
    public class StreamFilterTests : HostFixture
    {
        private readonly IStreamFilter filter;

        public StreamFilterTests() : base(services => services.AddStreamFilter())
        { 
            filter = GetRequiredService<IStreamFilter>();
        }

        [Fact]
        public void Stream_Write_()
        {
            var sslClient = new SslStream(filter.Stream);

            sslClient.AuthenticateAsClient("target-host");
        }
    }
}