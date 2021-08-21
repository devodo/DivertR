using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, string? name = null)
        {
            var decorateActions = CreateDecorateActions(services, diverter, name);

            var missingTypes = diverter.RegisteredVias(name)
                .Select(x => x.ViaId.Type)
                .Where(x => !decorateActions.ContainsKey(x))
                .Select(x => x.FullName)
                .ToArray();

            if (missingTypes.Length > 0)
            {
                throw new DiverterException(
                    $"Service type(s) not found: {string.Join(",", missingTypes)}");
            }

            foreach (var decorateAction in decorateActions.Values.SelectMany(x => x))
            {
                decorateAction.Invoke();
            }

            return services;
        }

        private static Dictionary<Type, IEnumerable<Action>> CreateDecorateActions(IServiceCollection services, IDiverter diverter, string? name)
        {
            var diverterTypes = new HashSet<Type>(diverter.RegisteredVias(name).Select(x => x.ViaId.Type));

            return services
                .Select((descriptor, index) =>
                {
                    if (!diverterTypes.Contains(descriptor.ServiceType))
                    {
                        return null;
                    }

                    var proxyFactory = CreateViaProxyFactory(descriptor, diverter, name);
                    void DecorateAction() => services[index] = ServiceDescriptor.Describe(descriptor.ServiceType, proxyFactory, descriptor.Lifetime);

                    return new
                    {
                        Type = descriptor.ServiceType,
                        Action = (Action) DecorateAction
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => x!.Type)
                .ToDictionary(grp => grp.Key, 
                    grp => grp.Select(x => x!.Action));
        }

        private static Func<IServiceProvider, object> CreateViaProxyFactory(ServiceDescriptor descriptor, IDiverter diverter, string? name)
        {
            var via = diverter.Via(descriptor.ServiceType, name);

            object ProxyFactory(IServiceProvider provider)
            {
                var instance = GetServiceInstance(provider, descriptor);
                return via.ProxyObject(instance);
            }

            return ProxyFactory;
        }
        
        private static object GetServiceInstance(IServiceProvider provider, ServiceDescriptor descriptor)
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