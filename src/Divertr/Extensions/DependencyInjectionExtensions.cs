using System;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection DiverterDecorate(this IServiceCollection serviceCollection, Diverter diverter)
        {
            diverter.Intercept(serviceCollection);

            return serviceCollection;
        }
        
        public static IServiceCollection DiverterDecorate<TStart>(this IServiceCollection serviceCollection, Diverter diverter)
        {
            diverter.Intercept<TStart>(serviceCollection);

            return serviceCollection;
        }
        
        public static IServiceCollection DiverterDecorate(this IServiceCollection serviceCollection, Diverter diverter, Type startType)
        {
            diverter.Intercept(serviceCollection, startType);

            return serviceCollection;
        }
        
        public static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
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