using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public interface IJsonClient
    {
        Task ConnectAsync(CancellationToken cancellationToken);

        Task DisconnectAsync(CancellationToken cancellationToken);

        Task SendObjectAsync(JObject @object, CancellationToken cancellationToken);

        Task SendObjectsAsync(IEnumerable<JObject> objects, CancellationToken cancellationToken);

        event EventHandler<ObjectReceivedEventArgs> ObjectReceived;
    }

    public class ObjectReceivedEventArgs : EventArgs
    {
        public ObjectReceivedEventArgs(JObject @object)
        {
            Object = @object;
        }

        public JObject Object { get; }
    }
}
