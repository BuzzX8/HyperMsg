using HyperMsg.Sockets;
using Newtonsoft.Json.Linq;
using System.Net;
using Xunit;

namespace HyperMsg.Integration
{
    public class JsonClientTests
    {
        private readonly ConfigurableBuilder<IJsonClient> clientBuilder;        
        private readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8080);

        public JsonClientTests()
        {            
            clientBuilder = new ConfigurableBuilder<IJsonClient>();
            clientBuilder.UseCoreServices<JObject>(2048, 2048);
            clientBuilder.UseSockets(endPoint);
            clientBuilder.UseJsonSerializer();
            clientBuilder.UseJsonClient();
        }

        [Fact]
        public void Connect_Establishes_Connection_With_Target()
        {
            var client = clientBuilder.Build();
            var acceptedClient = default(IJsonClient);

            client.Connect();

            Assert.NotNull(acceptedClient);
        }
    }
}
