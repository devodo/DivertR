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
        private readonly ConcurrentDictionary<string, ConcurrentQueue<IDiverterDecorator>> _decorators = new();
        private readonly ConcurrentDictionary<RedirectId, IRedirect> _registeredRedirects = new();
        private readonly ConcurrentDictionary<RedirectId, ConcurrentDictionary<RedirectId, IRedirect>> _registeredNested = new();

        /// <summary>
        /// Create a <see cref="DiverterBuilder"/> instance.
        /// </summary>
        /// <param name="settings">Optionally override default DivertR settings.</param>
        public DiverterBuilder(DiverterSettings? settings = null)
        {
            RedirectSet = new RedirectSet(settings);
        }
        
        /// <summary>
        /// Create a <see cref="DiverterBuilder"/> instance using an external <see cref="IRedirectSet"/>.
        /// </summary>
        /// <param name="redirectSet">The <see cref="IRedirectSet"/> instance.</param>
        public DiverterBuilder(IRedirectSet redirectSet)
        {
            RedirectSet = redirectSet;
        }
        
        /// <inheritdoc />
        public IRedirectSet RedirectSet { get; }

        /// <inheritdoc />
        public IDiverterBuilder Register<TTarget>(Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?
        {
            return Register<TTarget>(null, nestedRegisterAction);
        }

        /// <inheritdoc />
        public IDiverterBuilder Register<TTarget>(string? name, Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?
        {
            var redirect = RedirectSet.GetOrCreate<TTarget>(name);

            if (!_registeredRedirects.TryAdd(redirect.RedirectId, redirect))
            {
                throw new DiverterException($"Redirect already registered for {redirect.RedirectId}");
            }
            
            AddDecorator(name, new RedirectDecorator(redirect));

            nestedRegisterAction?.Invoke(new NestedRegisterBuilder<TTarget>(redirect, _registeredNested));

            return this;
        }

        /// <inheritdoc />
        public IDiverterBuilder Register(Type targetType, string? name = null)
        {
            var redirect = RedirectSet.GetOrCreate(targetType, name);
            
            if (!_registeredRedirects.TryAdd(redirect.RedirectId, redirect))
            {
                throw new DiverterException($"Redirect already registered for {redirect.RedirectId}");
            }
            
            AddDecorator(name, new RedirectDecorator(redirect));

            return this;
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
            AddDecorator(name, ServiceDecorator.Create(decorator));

            return this;
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(Type serviceType, Func<object, object> decorator)
        {
            return Decorate(null, serviceType, decorator);
        }
        
        /// <inheritdoc />
        public IDiverterBuilder Decorate(string? name, Type serviceType, Func<object, object> decorator)
        {
            AddDecorator(null, new ServiceDecorator(serviceType, decorator));
            
            return this;
        }
        
        /// <inheritdoc />
        public IDiverterBuilder AddRedirect<TTarget>(string? name = null) where TTarget : class?
        {
            RedirectSet.GetOrCreate<TTarget>(name);

            return this;
        }
        
        /// <inheritdoc />
        public IDiverterBuilder AddRedirect(Type type, string? name = null)
        {
            RedirectSet.GetOrCreate(type, name);

            return this;
        }

        /// <inheritdoc />
        public IDiverter Create()
        {
            IEnumerable<IDiverterDecorator> GetDecorators(string? name = null)
            {
                return _decorators.TryGetValue(name ?? string.Empty, out var decorators)
                    ? decorators
                    : Enumerable.Empty<IDiverterDecorator>();
            }
            
            return new Diverter(RedirectSet, GetDecorators);
        }

        private void AddDecorator(string? name, IDiverterDecorator decorator)
        {
            var decorators = _decorators.GetOrAdd(name ?? string.Empty, _ => new ConcurrentQueue<IDiverterDecorator>());
            decorators.Enqueue(decorator);
        }
    }
}