using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterCollection diverters, Action<RegistrationBuilder>? configureBuilder = null)
        {
            var builder = new RegistrationBuilder(services, diverters);
            configureBuilder?.Invoke(builder);
            builder.Build().Register();
            return services;
        }

        public static IServiceCollection Divert<T>(this IServiceCollection services, IDiverterCollection diverters, string? name = null) where T : class
        {
            return services.Divert(diverters, typeof(T), name);
        }
        
        public static IServiceCollection Divert(this IServiceCollection services, IDiverterCollection diverters, Type type, string? name = null)
        {
            return services.Divert(diverters, builder =>
            {
                builder.Include(type).WithName(name);
            });
        }

        public static IServiceCollection Divert<T>(this IServiceCollection services, IDiverter<T> diverter) where T : class
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

            return services;
        }

        internal static IEnumerable<Type> GetRange(this IServiceCollection services, Type? startType = null, Type? endType = null, bool startInclusive = true, bool endInclusive = true)
        {
            bool startFound = startType == null;
            foreach (var descriptor in services)
            {
                if (!startFound)
                {
                    if (descriptor.ServiceType != startType)
                    {
                        continue;
                    }

                    startFound = true;

                    if (!startInclusive)
                    {
                        continue;
                    }
                }

                if (!endInclusive && descriptor.ServiceType == endType)
                {
                    break;
                }

                if (descriptor.ServiceType.IsInterface && !descriptor.ServiceType.ContainsGenericParameters)
                {
                    yield return descriptor.ServiceType;
                }

                if (descriptor.ServiceType == endType)
                {
                    break;
                }
            }
        }
        
        internal static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
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