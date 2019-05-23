using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public interface IJsonClient
    {
        void Connect();

        Task ConnectAsync(CancellationToken cancellationToken);

        void Disconnect();

        Task DisconnectAsync(CancellationToken cancellationToken);

        void SendObject(JObject @object);

        Task SendObjectAsync(JObject @object, CancellationToken cancellationToken);

        void SendObjects(IEnumerable<JObject> objects);

        Task SendObjectsAsync(IEnumerable<JObject> objects);
    }
}
