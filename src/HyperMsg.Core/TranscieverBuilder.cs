using System;
using System.Buffers;
using System.Collections.Generic;

namespace HyperMsg
{
    public class TranscieverBuilder<T> : ITransceiverBuilder<T, T>
    {        
        private readonly List<Action<BuilderContext>> configurators;
        private readonly Func<ICollection<ServiceDescriptor>, IServiceProvider> serviceProviderFactory;

        public TranscieverBuilder(Func<ICollection<ServiceDescriptor>, IServiceProvider> serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<BuilderContext>>();
        }

        public void Configure(Action<BuilderContext> configurator) => configurators.Add(configurator);

        public ITransceiver<T, T> Build()
        {
            var context = new BuilderContext();

            foreach (var configurator in configurators)
            {
                configurator.Invoke(context);
            }            

            var serviceProvider = serviceProviderFactory.Invoke(context.Services);

            return CreateTransciever(serviceProvider, context.Runners);
        }

        private void RegisterTransciever(BuilderContext context)
        {
            var transceiver = ServiceDescriptor.Describe(typeof(ITransceiver<T, T>), typeof(MessageTransceiver<T>));
            context.Services.Add(transceiver);
        }

        private MessageTransceiver<T> CreateTransciever(IServiceProvider serviceProvider, ICollection<Func<IDisposable>> runners)
        {            
            var serializer = (ISerializer<T>)serviceProvider.GetService(typeof(ISerializer<T>));
            var stream = (IStream)serviceProvider.GetService(typeof(IStream));
            
            return new MessageTransceiver<T>(serializer, new Memory<byte>(), new Memory<byte>(), stream);
        }
    }
}
