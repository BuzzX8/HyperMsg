using Newtonsoft.Json.Linq;

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
                var client = new JsonClient();
                context.RegisterService(typeof(IJsonClient), client);
            });
        }
    }
}
