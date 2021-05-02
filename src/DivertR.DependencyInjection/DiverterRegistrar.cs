using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DivertR.DependencyInjection
{
    public class DiverterRegistrar
    {
        private readonly RegistrationConfiguration _configuration;
        private readonly Lazy<Dictionary<Type, List<Type>>> _openGenericTypes;

        private readonly HashSet<Type> _divertedTypesHash = new HashSet<Type>();
        private readonly List<Type> _divertedTypes = new List<Type>();
        private readonly List<ServiceDescriptor> _createDescriptors = new List<ServiceDescriptor>();

        private int _serviceIndex;

        public DiverterRegistrar(RegistrationConfiguration configuration)
        {
            _configuration = configuration;

            _openGenericTypes = new Lazy<Dictionary<Type, List<Type>>>(() =>
            {
                var openGenerics = _configuration.IncludeTypes
                    .Where(x => x.IsGenericType)
                    .Select(x => new
                    {
                        Original = x,
                        Generic = x.GetGenericTypeDefinition()
                    })
                    .GroupBy(x => x.Generic)
                    .ToDictionary(grp => grp.Key, 
                        grp => grp.Select(x => x.Original).ToList());

                return openGenerics;
            });
        }
        
        public void Register()
        {
            if (_serviceIndex > 0)
            {
                throw new InvalidOperationException("Register has already been called");
            }
            
            for (; _serviceIndex < _configuration.Services.Count; _serviceIndex++)
            {
                if (TryRegisterInclude(_serviceIndex))
                {
                    continue;
                }

                if (TryAddDescriptorForCreate(_serviceIndex))
                {
                    continue;
                }

                TryRegisterExclude(_serviceIndex);
            }

            RegisterCreateDescriptors();

            _configuration.RegistrationCallback?.Invoke(_divertedTypes);
        }

        private bool TryRegisterInclude(int servicesIndex)
        {
            var descriptor = _configuration.Services[servicesIndex];

            if (!_configuration.IncludeTypes.Contains(descriptor.ServiceType) ||
                _configuration.ExcludeTypes.Contains(descriptor.ServiceType))
            {
                return false;
            }

            InjectDiverter(servicesIndex, descriptor);

            return true;
        }
        
        private bool TryRegisterExclude(int servicesIndex)
        {
            var descriptor = _configuration.Services[servicesIndex];

            if (_configuration.IncludeTypes.Any())
            {
                return false;
            }

            if (!descriptor.ServiceType.IsInterface || descriptor.ServiceType.ContainsGenericParameters)
            {
                return false;
            }

            if (_configuration.ExcludeTypes.Contains(descriptor.ServiceType))
            {
                return false;
            }

            InjectDiverter(servicesIndex, descriptor);

            return true;
        }
        
        private void RegisterCreateDescriptors()
        {
            foreach (var descriptor in _createDescriptors.Where(x => !_divertedTypesHash.Contains(x.ServiceType)))
            {
                _configuration.Services.Add(descriptor);
                _divertedTypesHash.Add(descriptor.ServiceType);
                _divertedTypes.Add(descriptor.ServiceType);
            }
        }
        
        private bool TryAddDescriptorForCreate(int servicesIndex)
        {
            var descriptor = _configuration.Services[servicesIndex];

            if (!descriptor.ServiceType.IsGenericTypeDefinition)
            {
                return false;
            }
            
            if (!_openGenericTypes.Value.TryGetValue(descriptor.ServiceType, out var genericTypes))
            {
                return false;
            }
            
            foreach (var genericType in genericTypes)
            {
                if (_configuration.ExcludeTypes.Contains(genericType))
                {
                    continue;
                }
                        
                var implementationType = descriptor.ImplementationType.MakeGenericType(genericType.GetGenericArguments());
                        
                var via = _configuration.GetViaFunc(genericType);

                object ProxyFactory(IServiceProvider provider)
                {
                    var instance = ActivatorUtilities.GetServiceOrCreateInstance(provider, implementationType);
                    return via.ProxyObject(instance);
                }
                        
                _createDescriptors.Add(ServiceDescriptor.Describe(genericType, ProxyFactory, descriptor.Lifetime));
            }

            return true;
        }

        private void InjectDiverter(int servicesIndex, ServiceDescriptor descriptor)
        {
            var via = _configuration.GetViaFunc.Invoke(descriptor.ServiceType);

            object ProxyFactory(IServiceProvider provider)
            {
                var instance = GetInstance(provider, descriptor);
                return via.ProxyObject(instance);
            }
                
            _configuration.Services[servicesIndex] = ServiceDescriptor.Describe(descriptor.ServiceType, ProxyFactory, descriptor.Lifetime);
            _divertedTypesHash.Add(descriptor.ServiceType);
            _divertedTypes.Add(descriptor.ServiceType);
        }
        
        private static object GetInstance(IServiceProvider provider, ServiceDescriptor descriptor)
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
