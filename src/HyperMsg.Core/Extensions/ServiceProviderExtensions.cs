using System;
using System.Collections.Generic;
using System.Text;

namespace HyperMsg.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider, bool required = false) where T : class
        {
            var service = serviceProvider.GetService(typeof(T));

            if (service == null && required)
            {
                throw new InvalidOperationException();
            }

            if (service != null && !typeof(T).IsAssignableFrom(service.GetType()))
            {
                throw new InvalidOperationException();
            }

            return (T)service;
        }

        public static T GetRequiredService<T>(this IServiceProvider serviceProvider) where T : class => serviceProvider.GetService<T>(true);

        public static T GetOptionalService<T>(this IServiceProvider serviceProvider) where T : class => serviceProvider.GetService<T>(false);
    }
}
