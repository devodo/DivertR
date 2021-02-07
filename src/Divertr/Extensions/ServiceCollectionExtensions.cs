using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterSet diverterSet, IEnumerable<Type> types, string? name = null)
        {
            services.InjectDiverterSet(diverterSet, types, name);

            return services;
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterSet diverterSet, params Type[] types)
        {
            services.InjectDiverterSet(diverterSet, types);

            return services;
        }
        
        public static IServiceCollection Divert<T>(this IServiceCollection services, IDiverterSet diverterSet, string? name = null)
        {
            services.InjectDiverterSet(diverterSet, new[] {typeof(T)}, name);

            return services;
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterSet diverterSet, Type type, string? name = null)
        {
            services.InjectDiverterSet(diverterSet, new[] {type}, name);

            return services;
        }
        
        public static IServiceCollection Divert<T>(this IServiceCollection services, IDiverter<T> diverter) where T : class
        {
            services.InjectDiverter(diverter);

            return services;
        }

        private static void InjectDiverterSet(this IServiceCollection services, IDiverterSet diverterSet, IEnumerable<Type> types, string? name = null)
        {
            var typeHashSet = new HashSet<Type>(types);
            
            for (var i = 0; i < services.Count; i++)
            {
                var descriptor = services[i];

                if (!typeHashSet.Contains(descriptor.ServiceType))
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