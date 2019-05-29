using Newtonsoft.Json.Linq;

namespace HyperMsg.Integration
{
    public static class ConfigurableExtensions
    {
        public static void UseJsonSerializer(this IConfigurable configurable) => configurable.RegisterService(typeof(ISerializer<JObject>), (p, s) => new JsonSerializer());

        public static void UseJsonClient(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IJsonClient), (p, s) =>
            {
                var messageBuffer = (IMessageBuffer<JObject>)p.GetService(typeof(IMessageBuffer<JObject>));
                var handler = (IHandler)p.GetService(typeof(IHandler));
                var repository = (IHandlerRepository)p.GetService(typeof(IHandlerRepository));
                var client = new JsonClient(messageBuffer, handler);
                repository.AddHandler(client);

                return client;
            });
        }
    }
}
