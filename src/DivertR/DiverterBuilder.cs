using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR
{
    /// <inheritdoc />
    public class DiverterBuilder : IDiverterBuilder
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<IServiceDecorator>> _decorators = new();
        private readonly ConcurrentDictionary<RedirectId, IRedirect> _registeredRedirects = new();
        private readonly ConcurrentDictionary<RedirectId, ConcurrentDictionary<RedirectId, RedirectId>> _registeredNested = new();
        private readonly IDiverter _diverter;

        /// <summary>
        /// Create a <see cref="DiverterBuilder"/> instance.
        /// </summary>
        /// <param name="settings">Optionally override default DivertR settings.</param>
        public DiverterBuilder(DiverterSettings? settings = null)
            : this(new RedirectSet(settings))
        {
        }
        
        /// <summary>
        /// Create a <see cref="DiverterBuilder"/> instance using an external <see cref="IRedirectSet"/>.
        /// </summary>
        /// <param name="redirectSet">The <see cref="IRedirectSet"/> instance.</param>
        public DiverterBuilder(IRedirectSet redirectSet)
        {
            IEnumerable<IServiceDecorator> GetDecorators(string? name = null)
            {
                return _decorators.TryGetValue(name ?? string.Empty, out var decorators)
                    ? decorators
                    : Enumerable.Empty<IServiceDecorator>();
            }
            
            _diverter = new Diverter(redirectSet, GetDecorators);
        }

        /// <inheritdoc />
        public IDiverterBuilder Register<TTarget>(string? name = null) where TTarget : class?
        {
            var redirect = _diverter.RedirectSet.GetOrCreate<TTarget>(name);

            if (!_registeredRedirects.TryAdd(redirect.RedirectId, redirect))
            {
                throw new DiverterException($"Redirect already registered for {redirect.RedirectId}");
            }

            return Decorate(name, new RedirectDecorator(redirect));
        }

        /// <inheritdoc />
        public IDiverterBuilder Register(Type targetType, string? name = null)
        {
            var redirect = _diverter.RedirectSet.GetOrCreate(targetType, name);
            
            if (!_registeredRedirects.TryAdd(redirect.RedirectId, redirect))
            {
                throw new DiverterException($"Redirect already registered for {redirect.RedirectId}");
            }
            
            return Decorate(name, new RedirectDecorator(redirect));
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Register(IEnumerable<Type> types, string? name = null)
        {
            foreach (var type in types)
            {
                Register(type, name);
            }

            return this;
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Register(params Type[] types)
        {
            return Register(types, null);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Register(string? name, params Type[] types)
        {
            return Register(types, name);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate<TService>(Func<TService, TService> decorator)
        {
            return Decorate(null, decorator);
        }

        /// <inheritdoc />
        public IDiverterBuilder Decorate<TService>(string? name, Func<TService, TService> decorator)
        {
            return Decorate(name, ServiceDecorator.Create(decorator));
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate<TService>(Func<TService, IDiverter, TService> decorator)
        {
            return Decorate(null, decorator);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate<TService>(string? name, Func<TService, IDiverter, TService> decorator)
        {
            return Decorate(name, ServiceDecorator.Create(decorator));
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate<TService>(Func<TService, IDiverter, IServiceProvider, TService> decorator)
        {
            return Decorate(null, decorator);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate<TService>(string? name, Func<TService, IDiverter, IServiceProvider, TService> decorator)
        {
            return Decorate(name, ServiceDecorator.Create(decorator));
        }

        /// <inheritdoc />
        public IDiverterBuilder Decorate(Type serviceType, Func<object, object> decorator)
        {
            return Decorate(null, serviceType, decorator);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(string? name, Type serviceType, Func<object, object> decorator)
        {
            return Decorate(name, ServiceDecorator.Create(serviceType, decorator));
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(Type serviceType, Func<object, IDiverter, object> decorator)
        {
            return Decorate(null, serviceType, decorator);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(string? name, Type serviceType, Func<object, IDiverter, object> decorator)
        {
            return Decorate(name, ServiceDecorator.Create(serviceType, decorator));
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(Type serviceType, Func<object, IDiverter, IServiceProvider, object> decorator)
        {
            return Decorate(null, serviceType, decorator);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(string? name, Type serviceType, Func<object, IDiverter, IServiceProvider, object> decorator)
        {
            return Decorate(name, ServiceDecorator.Create(serviceType, decorator));
        }

        /// <inheritdoc />
        public IDiverterBuilder AddRedirect<TTarget>(string? name = null) where TTarget : class?
        {
            _diverter.RedirectSet.GetOrCreate<TTarget>(name);

            return this;
        }
        
        /// <inheritdoc />
        public IDiverterBuilder AddRedirect(Type type, string? name = null)
        {
            _diverter.RedirectSet.GetOrCreate(type, name);

            return this;
        }
        
        /// <inheritdoc />
        public IDiverterRedirectBuilder<TTarget> ExtendRedirect<TTarget>(string? name = null) where TTarget : class?
        {
            var redirect = _diverter.Redirect<TTarget>();
            
            return new DiverterRedirectBuilder<TTarget>(redirect, this);
        }

        /// <inheritdoc />
        public IDiverter Create()
        {
            return _diverter;
        }

        internal bool TryAddNestedRedirect(RedirectId parentRedirectId, RedirectId nestedRedirectId)
        {
            var registered = _registeredNested.GetOrAdd(parentRedirectId,
                _ => new ConcurrentDictionary<RedirectId, RedirectId>());

            return registered.TryAdd(nestedRedirectId, nestedRedirectId);
        }
        
        internal ICallHandler CreateDecoratorHandler<TReturn>(Func<TReturn, IDiverter, TReturn> decorator)
        {
            return new RedirectDecoratorCallHandler<TReturn>(_diverter, decorator);
        }
        
        internal static Action<IViaOptionsBuilder> GetOptions()
        {
            return opt =>
            {
                opt.DisableSatisfyStrict();
                opt.Persist();
            };
        }
        
        private IDiverterBuilder Decorate(string? name, IServiceDecorator decorator)
        {
            var decorators = _decorators.GetOrAdd(name ?? string.Empty, _ => new ConcurrentQueue<IServiceDecorator>());
            decorators.Enqueue(decorator);

            return this;
        }
    }
}