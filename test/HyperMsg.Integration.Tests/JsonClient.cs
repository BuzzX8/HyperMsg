using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HyperMsg.Integration
{
    public class JsonClient : IJsonClient
    {
        public void Connect()
        {
            throw new System.NotImplementedException();
        }

        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public Task DisconnectAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void SendObject(JObject @object)
        {
            throw new System.NotImplementedException();
        }

        public Task SendObjectAsync(JObject @object, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void SendObjects(IEnumerable<JObject> objects)
        {
            throw new System.NotImplementedException();
        }

        public Task SendObjectsAsync(IEnumerable<JObject> objects)
        {
            throw new System.NotImplementedException();
        }
    }
}
