namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseTransciever<T>(this IConfigurable configurable)
        {
            configurable.Configure(context => 
            {
                var receiver = (IReceiver<T>)context.GetService(typeof(IReceiver<T>));
                var sender = (ISender<T>)context.GetService(typeof(ISender<T>));
                var transciever = new MessageTransceiver<T>(receiver, sender);

                context.RegisterService(typeof(ITransceiver<T, T>), transciever);
            });
        }
    }
}
