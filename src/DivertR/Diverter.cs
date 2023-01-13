using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DivertR
{
    /// <inheritdoc />
    public class Diverter : IDiverter
    {
        private readonly ConcurrentDictionary<RedirectId, IRedirect> _registeredRedirects = new();
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
        public IDiverter Register<TTarget>(string? name = null) where TTarget : class?
        {
            var redirect = RedirectSet.GetOrCreate<TTarget>(name);

            if (!_registeredRedirects.TryAdd(redirect.RedirectId, redirect))
            {
                throw new DiverterException($"Redirect already registered for {redirect.RedirectId}");
            }

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
        public IDiverter Register(string name, params Type[] types)
        {
            return Register(types, name);
        }

        /// <inheritdoc />
        public IEnumerable<IRedirect> RegisteredRedirects(string? name = null)
        {
            name ??= string.Empty;
            
            return _registeredRedirects
                .Where(x => x.Key.Name == name)
                .Select(x => x.Value);
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
        public IDiverter ResetAll(bool includePersistent = false)
        {
            RedirectSet.ResetAll(includePersistent);
            
            return this;
        }
        
        /// <inheritdoc />
        public IDiverter Reset(string? name = null, bool includePersistent = false)
        {
            RedirectSet.Reset(name, includePersistent);

            return this;
        }
    }
}