﻿using System;
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
                    
                    void DecorateAction()
                    {
                        var proxyFactory = CreateViaProxyFactory(services, descriptor, via);
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
        
        private static Func<IServiceProvider, object?> CreateViaProxyFactory(IServiceCollection services, ServiceDescriptor descriptor, IVia via)
        {
            var rootFactory = CreateRootFactory(services, descriptor);
            
            return provider =>
            {
                var root = rootFactory.Invoke(provider);
                
                return via.ViaSet.Settings.DiverterProxyDecorator.Decorate(via, root);
            };
        }
        
        private static Func<IServiceProvider, object?> CreateRootFactory(IServiceCollection services, ServiceDescriptor descriptor)
        {
            var isDisposable = typeof(IDisposable).IsAssignableFrom(descriptor.ServiceType) ||
                               typeof(IAsyncDisposable).IsAssignableFrom(descriptor.ServiceType);
            
            return !isDisposable
                ? CreateDisposingRootFactory(services, descriptor)
                : CreateNonDisposingRootFactory(descriptor);
        }

        private static Func<IServiceProvider, object?> CreateDisposingRootFactory(IServiceCollection services, ServiceDescriptor descriptor)
        {
            var typePointer = new TypePointer(descriptor.ServiceType);
            
            var pointerDescriptor = descriptor switch
            {
                { ImplementationType: not null } => new ServiceDescriptor(typePointer, descriptor.ImplementationType, descriptor.Lifetime),
                { ImplementationFactory: not null } => new ServiceDescriptor(typePointer, descriptor.ImplementationFactory, descriptor.Lifetime),
                { ImplementationInstance: not null } => new ServiceDescriptor(typePointer, descriptor.ImplementationInstance),
                _ => throw new ArgumentException($"No ServiceDescriptor implementation defined on {descriptor.ServiceType}", nameof(descriptor))
            };
            
            services.Add(pointerDescriptor);

            return provider => provider.GetService(typePointer);
        }
        
        private static Func<IServiceProvider, object?> CreateNonDisposingRootFactory(ServiceDescriptor descriptor)
        {
            return descriptor switch
            {
                { ImplementationType: not null } => provider => ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType),
                { ImplementationFactory: not null } => descriptor.ImplementationFactory,
                { ImplementationInstance: not null } => _ => descriptor.ImplementationInstance,
                _ => throw new ArgumentException($"No ServiceDescriptor implementation defined on {descriptor.ServiceType}", nameof(descriptor))
            };
        }
    }
}
