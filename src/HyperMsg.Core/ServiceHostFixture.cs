﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace HyperMsg
{
    public abstract class ServiceHostFixture : IDisposable
    {
        private readonly ServiceHost serviceHost;

        protected ServiceHostFixture(Action<IServiceCollection> serviceConfigurator = null)
        {
            serviceHost = ServiceHost.CreateDefault(serviceConfigurator);
            serviceHost.Start();
        }

        protected IMessagingContext MessagingContext => GetRequiredService<IMessagingContext>();

        protected IMessageSender MessageSender => GetRequiredService<IMessageSender>();

        protected IMessageHandlersRegistry HandlersRegistry => GetRequiredService<IMessageHandlersRegistry>();

        protected T GetService<T>() => serviceHost.GetService<T>();

        protected T GetRequiredService<T>() => serviceHost.GetRequiredService<T>();

        public virtual void Dispose()
        {
            serviceHost.Stop();
            serviceHost.Dispose();
        }
    }
}