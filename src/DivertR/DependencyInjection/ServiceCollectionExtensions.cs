using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.DependencyInjection
{
    /// <summary>
    /// Extension methods for installing an <see cref="IDiverter"/> instance into an <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection">IServiceCollection</see> container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Installs the <see cref="IRedirect"/> collection of registered types from the <paramref name="diverter"/> instance into the <paramref name="services"/> container.
        /// The Redirects are embedded by decorating existing container registrations, of matching types, with proxy factories that produce <see cref="IRedirect"/> proxies wrapping the originals as their root instances.
        /// </summary>
        /// <param name="services">The services container.</param>
        /// <param name="diverter">The Diverter instance.</param>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the registered <see cref="IRedirect"/> group.</param>
        /// <returns>The input services container.</returns>
        /// <exception cref="DiverterException">Throws if either the <paramref name="diverter"/> instances contains registered types not in the <paramref name="services"/> or vice versa.</exception>
        public static IServiceCollection Divert(this IServiceCollection services, IDiverter diverter, string? name = null)
        {
            var decorateActions = CreateDecorateActions(services, diverter, name);

            var missingTypes = diverter.RegisteredRedirects(name)
                .Select(x => x.RedirectId.Type)
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
            var registeredRedirects = diverter.RegisteredRedirects(name).ToDictionary(x => x.RedirectId.Type);

            return services
                .Select((descriptor, index) =>
                {
                    if (descriptor.ServiceType is TypePointer)
                    {
                        return null;
                    }
                    
                    if (!registeredRedirects.TryGetValue(descriptor.ServiceType, out var redirect))
                    {
                        return null;
                    }
                    
                    void DecorateAction()
                    {
                        var proxyFactory = CreateRedirectProxyFactory(services, descriptor, redirect);
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
        
        private static Func<IServiceProvider, object?> CreateRedirectProxyFactory(IServiceCollection services, ServiceDescriptor descriptor, IRedirect redirect)
        {
            var rootFactory = CreateRootFactory(services, descriptor);
            
            return provider =>
            {
                var root = rootFactory.Invoke(provider);
                
                return redirect.RedirectSet.Settings.DiverterProxyDecorator.Decorate(redirect, root);
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
