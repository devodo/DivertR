using System;
using System.Collections.Generic;
using System.Linq;
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
            var registeredVias = diverter.RegisteredVias(name).ToDictionary(x => x.ViaId.Type);

            return services
                .Select((descriptor, index) =>
                {
                    if (descriptor.ServiceType is TypePointer)
                    {
                        return null;
                    }
                    
                    if (!registeredVias.TryGetValue(descriptor.ServiceType, out var via))
                    {
                        return null;
                    }
                    
                    var typePointer = new TypePointer(descriptor.ServiceType);
                    var proxyFactory = CreateViaProxyFactory(typePointer, via);

                    void DecorateAction()
                    {
                        services.Add(descriptor.ToPointerDescriptor(typePointer));
                        services[index] = new ServiceDescriptor(descriptor.ServiceType, proxyFactory, descriptor.Lifetime);
                    }

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

        private static ServiceDescriptor ToPointerDescriptor(this ServiceDescriptor original, Type typePointer)
        {
            if (original.ImplementationType != null)
            {
                return new ServiceDescriptor(typePointer, original.ImplementationType, original.Lifetime);
            }

            if (original.ImplementationFactory != null)
            {
                return new ServiceDescriptor(typePointer, original.ImplementationFactory, original.Lifetime);
            }

            if (original.ImplementationInstance != null)
            {
                return new ServiceDescriptor(typePointer, original.ImplementationInstance);
            }

            throw new ArgumentException($"No ServiceDescriptor implementation defined on {original.ServiceType}", nameof(original));
        }
        
        private static Func<IServiceProvider, object?> CreateViaProxyFactory(Type typePointer, IVia via)
        {
            object? ProxyFactory(IServiceProvider provider)
            {
                var instance = provider.GetService(typePointer);
                return via.ViaSet.Settings.DependencyFactory.Create(via, instance);
            }

            return ProxyFactory;
        }
    }
}