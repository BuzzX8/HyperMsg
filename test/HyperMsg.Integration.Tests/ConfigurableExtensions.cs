using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HyperMsg.Integration
{
    public static class ConfigurableExtensions
    {
        public static void UseJsonSerializer(this IConfigurable configurable)
        {
            configurable.Configure(context =>
            {
                context.RegisterService(typeof(ISerializer<JObject>), new JsonSerializer());
            });
        }

        public static void UseJsonClient(this IConfigurable configurable)
        {
            configurable.Configure(context =>
            {
                var messageBuffer = (IMessageBuffer<JObject>)context.GetService(typeof(IMessageBuffer<JObject>));
                var transportHandler = (IHandler<TransportCommands>)context.GetService(typeof(IHandler<TransportCommands>));
                var receiveModeHandler = (IHandler<ReceiveMode>)context.GetService(typeof(IHandler<ReceiveMode>));
                var client = new JsonClient(messageBuffer, transportHandler, receiveModeHandler);
                //var handlerCollection = (ICollection<IHandler<JObject>>)context.GetService(typeof(ICollection<IHandler<JObject>>));
                //handlerCollection.Add(client);
                context.RegisterService(typeof(IJsonClient), client);
            });
        }
    }
}
