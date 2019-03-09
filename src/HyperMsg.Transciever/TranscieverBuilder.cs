using System;
using System.Buffers;
using System.Collections.Generic;

namespace HyperMsg.Transciever
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
            var memoryOwner = (IMemoryOwner<byte>)serviceProvider.GetService(typeof(IMemoryOwner<byte>));

            var messageReader = new MessageReader<T>(serializer.Deserialize);
            var writer = new ByteBufferWriter(memoryOwner);
            //var messageBuffer = new MessageBuffer<T>(writer, serializer.Serialize);            
            //var transciever = new MessageTransceiver<T>(messageBuffer, messageReader.SetMessageHandler, runners);

            return null;// transciever;
        }
    }
}
