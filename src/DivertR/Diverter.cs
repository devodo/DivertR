using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DivertR.Internal;

namespace DivertR
{
    /// <inheritdoc />
    public class Diverter : IDiverter
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<IDiverterDecorator>> _decorators = new();
        private readonly ConcurrentDictionary<RedirectId, IRedirect> _registeredRedirects = new();
        private readonly ConcurrentDictionary<RedirectId, ConcurrentDictionary<RedirectId, IRedirect>> _registeredNested = new();
        public IRedirectSet RedirectSet { get; }

        /// <summary>
        /// Create a <see cref="Diverter"/> instance.
        /// </summary>
        /// <param name="settings">Optionally override default DivertR settings.</param>
        public Diverter(DiverterSettings? settings = null)
        {
            RedirectSet = new RedirectSet(settings);
        }
        
        /// <summary>
        /// Create a <see cref="Diverter"/> instance using an external <see cref="IRedirectSet"/>.
        /// </summary>
        /// <param name="redirectSet">The <see cref="IRedirectSet"/> instance.</param>
        public Diverter(IRedirectSet redirectSet)
        {
            RedirectSet = redirectSet;
        }
        
        /// <inheritdoc />
        public IDiverter Register<TTarget>(Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?
        {
            return Register<TTarget>(null, nestedRegisterAction);
        }
        
        /// <inheritdoc />
        public IDiverter Register<TTarget>(string? name, Action<INestedRegisterBuilder>? nestedRegisterAction = null) where TTarget : class?
        {
            var redirect = RedirectSet.GetOrCreate<TTarget>(name);

            if (!_registeredRedirects.TryAdd(redirect.RedirectId, redirect))
            {
                throw new DiverterException($"Redirect already registered for {redirect.RedirectId}");
            }
            
            AddDecorator(name, new RedirectDecorator(redirect));

            nestedRegisterAction?.Invoke(new NestedRegisterBuilder<TTarget>(RedirectSet.Get<TTarget>(name)!, _registeredNested));

            return this;
        }

        /// <inheritdoc />
        public IDiverter Register(Type targetType, string? name = null)
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
        public IDiverter Register(IEnumerable<Type> types, string? name = null)
        {
            foreach (var type in types)
            {
                Register(type, name);
            }

            return this;
        }
        
        /// <inheritdoc />
        public IDiverter Register(params Type[] types)
        {
            return Register(types, null);
        }
        
        /// <inheritdoc />
        public IDiverter Register(string? name, params Type[] types)
        {
            return Register(types, name);
        }
        
        /// <inheritdoc />
        public IDiverter Decorate<TService>(Func<TService, TService> decorator)
        {
            return Decorate(null, decorator);
        }

        /// <inheritdoc />
        public IDiverter Decorate<TService>(string? name, Func<TService, TService> decorator)
        {
            AddDecorator(name, ServiceDecorator.Create(decorator));

            return this;
        }
        
        /// <inheritdoc />
        public IDiverter Decorate(Type serviceType, Func<object, object> decorator)
        {
            return Decorate(null, serviceType, decorator);
        }
        
        /// <inheritdoc />
        public IDiverter Decorate(string? name, Type serviceType, Func<object, object> decorator)
        {
            AddDecorator(null, new ServiceDecorator(serviceType, decorator));
            
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<IDiverterDecorator> GetDecorators(string? name = null)
        {
            return _decorators.TryGetValue(name ?? string.Empty, out var decorators)
                ? decorators
                : Enumerable.Empty<IDiverterDecorator>();
        }

        /// <inheritdoc />
        public IRedirect<TTarget> Redirect<TTarget>(string? name = null) where TTarget : class?
        {
            return (IRedirect<TTarget>) Redirect(RedirectId.From<TTarget>(name));
        }
        
        /// <inheritdoc />
        public IRedirect Redirect(Type type, string? name = null)
        {
            return Redirect(RedirectId.From(type, name));
        }
        
        /// <inheritdoc />
        public IRedirect Redirect(RedirectId redirectId)
        {
            var redirect = RedirectSet.Get(redirectId);
            
            if (redirect == null)
            {
                throw new DiverterException($"Redirect not registered for {redirectId}");
            }
            
            return redirect;
        }

        /// <inheritdoc />
        public IDiverter StrictAll()
        {
            RedirectSet.StrictAll();
            
            return this;
        }

        /// <inheritdoc />
        public IDiverter Strict(string? name = null)
        {
            RedirectSet.Strict(name);

            return this;
        }
        
        /// <inheritdoc />
        public IDiverter ResetAll()
        {
            RedirectSet.ResetAll();
            
            return this;
        }
        
        /// <inheritdoc />
        public IDiverter Reset(string? name = null)
        {
            RedirectSet.Reset(name);

            return this;
        }
        
        private void AddDecorator(string? name, IDiverterDecorator decorator)
        {
            var decorators = _decorators.GetOrAdd(name ?? string.Empty, _ => new ConcurrentQueue<IDiverterDecorator>());
            decorators.Enqueue(decorator);
        }
    }
}