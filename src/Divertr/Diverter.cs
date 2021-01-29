using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Divertr.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Divertr
{
    public class Diverter
    {
        private const BindingFlags ActivatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        
        private readonly DiversionStore _diversionStore = new DiversionStore();
        private readonly ConcurrentDictionary<DiversionId, object> _diverters = new ConcurrentDictionary<DiversionId, object>();

        public IDiverter<T> Of<T>(string name = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            return (IDiverter<T>) _diverters.GetOrAdd(DiversionId.From<T>(name),
                id => new Diverter<T>(id, _diversionStore));
        }

        public IDiverter Of(Type type, string name = null)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", nameof(type));
            }

            return (IDiverter) _diverters.GetOrAdd(DiversionId.From(type, name),
                id =>
                {
                    var diverterType = typeof(Diverter<>).MakeGenericType(type);
                    return Activator.CreateInstance(diverterType, ActivatorFlags, null, new object[] {id, _diversionStore}, default(CultureInfo));
                });
        }
        
        public void ResetAll()
        {
            _diversionStore.Reset();
        }

        public List<ServiceDescriptor> Intercept(IServiceCollection services, string name = null)
        {
            return InterceptImpl(services, null, name);
        }
        
        public List<ServiceDescriptor> Intercept<TStart>(IServiceCollection services, string name = null)
        {
            return InterceptImpl(services, typeof(TStart), name);
        }
        
        public List<ServiceDescriptor> Intercept(IServiceCollection services, Type startType, string name = null)
        {
            return InterceptImpl(services, startType, name);
        }
        
        private List<ServiceDescriptor> InterceptImpl(IServiceCollection services, Type startType, string name)
        {
            var interceptedDescriptors = new List<ServiceDescriptor>();
            bool startTypeFound = false;
            
            for (var i = 0; i < services.Count; i++)
            {
                var descriptor = services[i];

                if (!startTypeFound)
                {
                    if (startType != null && descriptor.ServiceType != startType)
                    {
                        continue;
                    }
                    
                    startTypeFound = true;
                }
                
                if (!descriptor.ServiceType.IsInterface)
                {
                    continue;
                }

                var diverter = Of(descriptor.ServiceType, name);
                object ProxyFactory(IServiceProvider provider)
                {
                    var instance = provider.GetInstance(descriptor);
                    return diverter.Proxy(instance);
                }
                
                services[i] = ServiceDescriptor.Describe(descriptor.ServiceType, ProxyFactory, descriptor.Lifetime);
                interceptedDescriptors.Add(descriptor);
            }

            return interceptedDescriptors;
        }
    }

    public class Diverter<T> : IDiverter<T> where T : class
    {
        private readonly DiversionId _diversionId;
        private readonly DiversionStore _diversionStore;
        private readonly Lazy<CallContext<T>> _callContext;

        public Diverter() : this(DiversionId.From<T>(), new DiversionStore())
        {
        }

        internal Diverter(DiversionId diversionId, DiversionStore diversionStore)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }
            
            _diversionId = diversionId;
            _diversionStore = diversionStore;
            _callContext = new Lazy<CallContext<T>>(() => new CallContext<T>());
        }

        public ICallContext<T> CallContext => _callContext.Value;

        public IServiceCollection Intercept(IServiceCollection services)
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
                    return Proxy((T)instance);
                }

                services[i] = ServiceDescriptor.Describe(descriptor.ServiceType, ProxyFactory, descriptor.Lifetime);
            }

            return services;
        }
        public T Proxy(T origin = null)
        {
            Diversion<T> GetDiversion()
            {
                return _diversionStore.GetDiversion<T>(_diversionId);
            }

            return ProxyFactory.Instance.CreateInstanceProxy(origin, GetDiversion);
        }
        
        public object Proxy(object origin)
        {
            if (origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            if (!(origin is T))
            {
                throw new ArgumentException($"Not assignable to {typeof(T).Name}", nameof(origin));
            }

            Diversion<T> GetDiversion()
            {
                return _diversionStore.GetDiversion<T>(_diversionId);
            }

            return ProxyFactory.Instance.CreateInstanceProxy((T)origin, GetDiversion);
        }

        public IDiverter<T> Redirect(T redirect)
        {
            var diversion = new Diversion<T>(new Redirection<T>(redirect), _callContext.Value);
            _diversionStore.SetDiversion(_diversionId, diversion);

            return this;
        }

        public IDiverter<T> AddRedirect(T redirect)
        {
            Diversion<T> Create()
            {
                return new Diversion<T>(new Redirection<T>(redirect), _callContext.Value);
            }

            Diversion<T> Update(Diversion<T> existingDiversion)
            {
                return existingDiversion.AppendRedirection(new Redirection<T>(redirect));
            }

            _diversionStore.AddOrUpdateDiversion(_diversionId, Create, Update);

            return this;
        }

        public IDiverter<T> Reset()
        {
            _diversionStore.Reset(_diversionId);

            return this;
        }
    }
}