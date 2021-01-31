using System;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterSet diverterSet, string name = null)
        {
            services.InjectDiverterSet(diverterSet, name: name);

            return services;
        }
        
        public static IServiceCollection Divert<TStart>(this IServiceCollection services, IDiverterSet diverterSet, string name = null)
        {
            services.InjectDiverterSet(diverterSet, typeof(TStart), name);

            return services;
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterSet diverterSet, Type startType, string name = null)
        {
            services.InjectDiverterSet(diverterSet, startType, name);

            return services;
        }
        
        public static IServiceCollection Divert<T>(this IServiceCollection services, IDiverter<T> diverter) where T : class
        {
            services.InjectDiverter(diverter);

            return services;
        }

        private static void InjectDiverterSet(this IServiceCollection services, IDiverterSet diverterSet, Type startType = null, string name = null)
        {
            bool startTypeFound = startType == null;
            
            for (var i = 0; i < services.Count; i++)
            {
                var descriptor = services[i];

                if (!startTypeFound)
                {
                    if (descriptor.ServiceType != startType)
                    {
                        continue;
                    }
                    
                    startTypeFound = true;
                }
                
                if (!descriptor.ServiceType.IsInterface)
                {
                    continue;
                }

                var diverter = diverterSet.Get(descriptor.ServiceType, name);
                object ProxyFactory(IServiceProvider provider)
                {
                    var instance = provider.GetInstance(descriptor);
                    return diverter.Proxy(instance);
                }
                
                services[i] = ServiceDescriptor.Describe(descriptor.ServiceType, ProxyFactory, descriptor.Lifetime);
            }
        }
        
        private static void InjectDiverter<T>(this IServiceCollection services, IDiverter<T> diverter) where T : class
        {
            for (var i = 0; i < services.Count; i++)
            {
                var descriptor = services[i];
                
                if (descriptor.ServiceType != typeof(T))
                {
                    continue;
                }

                object ProxyFactory(IServiceProvider provider)
                {
                    var instance = provider.GetInstance(descriptor);
                    return diverter.Proxy((T)instance);
                }

                services[i] = ServiceDescriptor.Describe(descriptor.ServiceType, ProxyFactory, descriptor.Lifetime);
            }
        }
        
        private static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationType != null)
            {
                return ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType);
            }

            return descriptor.ImplementationFactory(provider);
        }
    }
}