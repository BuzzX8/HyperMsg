using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HyperMsg.Integration
{
    public static class ConfigurableExtensions
    {
        public static void UseJsonSerializer(this IConfigurable configurable)
        {
            configurable.AddService(typeof(ISerializer<JObject>), (p, s) => new JsonSerializer());
        }

        public static void UseJsonClient(this IConfigurable configurable)
        {
            configurable.AddService(typeof(IJsonClient), (p, s) =>
            {
                var messageBuffer = (IMessageBuffer<JObject>)p.GetService(typeof(IMessageBuffer<JObject>));
                var transportHandler = (IHandler<TransportCommands>)p.GetService(typeof(IHandler<TransportCommands>));
                var receiveModeHandler = (IHandler<ReceiveMode>)p.GetService(typeof(IHandler<ReceiveMode>));
                var handlerCollection = (ICollection<IHandler<JObject>>)p.GetService(typeof(ICollection<IHandler<JObject>>));

                var client = new JsonClient(messageBuffer, transportHandler, receiveModeHandler);
                handlerCollection.Add(client);

                return client;
            });
        }
    }
}
