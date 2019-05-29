﻿using System.Collections.Generic;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseCoreServices<T>(this IConfigurable configurable, int inputBufferSize, int outputBufferSize)
        {
            configurable.UseHandlerRepository<T>();
            configurable.UseCompositeHandler();
            configurable.UseBackgrounReceiver<T>();
            configurable.UseBufferReader(inputBufferSize);
            configurable.UseMessageReceiver<T>();
            configurable.UseMessageBuffer<T>(outputBufferSize);
            configurable.UseTransciever<T>();
        }

        public static void UseTransciever<T>(this IConfigurable configurable)
        {
            configurable.AddService(typeof(ITransceiver<T, T>), (p, s) =>
            {
                var receiver = (IReceiver<T>)p.GetService(typeof(IReceiver<T>));
                var sender = (ISender<T>)p.GetService(typeof(ISender<T>));
                return new MessageTransceiver<T>(receiver, sender);
            });
        }

        public static void UseMessageBuffer<T>(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "MessageBuffer.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.AddService(new[] { typeof(ISender<T>), typeof(IMessageBuffer<T>) }, (p, s) =>
            {
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var stream = (IStream)p.GetService(typeof(IStream));
                var buffSize = (int)s[SettingName];
                return new MessageBuffer<T>(serializer.Serialize, new byte[bufferSize], stream.WriteAsync);
            });
        }

        public static void UseMessageReceiver<T>(this IConfigurable configurable)
        {
            configurable.AddService(typeof(IReceiver<T>), (p, s) =>
            {
                var reader = (IBufferReader)p.GetService(typeof(IBufferReader));
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                return new MessageReceiver<T>(serializer.Deserialize, reader);
            });
        }

        public static void UseBufferReader(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "BufferReader.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.AddService(typeof(IBufferReader), (p, s) =>
            {
                var buffSize = (int)s[SettingName];
                var stream = (IStream)p.GetService(typeof(IStream));
                return new BufferReader(new byte[buffSize], stream.ReadAsync);
            });
        }

        public static void UseBackgrounReceiver<T>(this IConfigurable configurable)
        {
            configurable.AddService(typeof(IHandler<ReceiveMode>), (p, s) =>
            {
                var receiver = (IReceiver<T>)p.GetService(typeof(IReceiver<T>));
                var repository = (IHandlerRepository)p.GetService(typeof(IHandlerRepository));
                var bgReceiver = new BackgroundReceiver<T>(receiver, repository.GetHandlers<T>);
                repository.AddHandler(bgReceiver);

                return bgReceiver;
            });
        }

        public static void UseCompositeHandler(this IConfigurable configurable) => configurable.AddService(typeof(IHandler), (p, s) =>
        {
            var repository = (IHandlerRepository)p.GetService(typeof(IHandlerRepository));
            return new CompositeHandler(repository);
        });

        public static void UseHandlerRepository<T>(this IConfigurable configurable) => configurable.AddService(typeof(IHandlerRepository), (p, s) => new HandlerRepository());
    }
}
