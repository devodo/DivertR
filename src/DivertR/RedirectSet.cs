using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DivertR
{
    /// <inheritdoc />
    public class RedirectSet : IRedirectSet
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, IRedirect>> _redirectGroups = new();
        
        public RedirectSet(DiverterSettings? settings = null)
        {
            Settings = settings ?? DiverterSettings.Global;
        }
        
        /// <inheritdoc />
        public DiverterSettings Settings { get; }
        
        /// <inheritdoc />
        public IRedirect<TTarget> GetOrCreate<TTarget>(string? name = null) where TTarget : class?
        {
            var redirectId = RedirectId.From<TTarget>(name);
            var redirectGroup = GetRedirectGroup(redirectId.Name);
            var redirect = redirectGroup.GetOrAdd(redirectId.Type, _ => new Redirect<TTarget>(redirectId, this));

            return (IRedirect<TTarget>) redirect;
        }
        
        /// <inheritdoc />
        public IRedirect GetOrCreate(Type type, string? name = null)
        {
            var redirectId = RedirectId.From(type, name);

            return GetOrCreate(redirectId);
        }
        
        /// <inheritdoc />
        public IRedirect GetOrCreate(RedirectId redirectId)
        {
            IRedirect CreateRedirect(Type type)
            {
                const BindingFlags ActivatorFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                
                var diverterType = typeof(Redirect<>).MakeGenericType(type);
                var constructorParams = new object[] { redirectId, this, null! };
                var redirect = (IRedirect) Activator.CreateInstance(diverterType, ActivatorFlags, null, constructorParams, default);

                return redirect;
            }
            
            var redirectGroup = GetRedirectGroup(redirectId.Name);
            
            return redirectGroup.GetOrAdd(redirectId.Type, CreateRedirect);
        }

        /// <inheritdoc />
        public IRedirect<TTarget>? Get<TTarget>(string? name = null) where TTarget : class?
        {
            var redirectId = RedirectId.From<TTarget>(name);
            var redirect = Get(redirectId);

            return (IRedirect<TTarget>?) redirect;
        }
        
        /// <inheritdoc />
        public IRedirect? Get(Type type, string? name = null)
        {
            var redirectId = RedirectId.From(type, name);

            return Get(redirectId);
        }
        
        /// <inheritdoc />
        public IRedirect? Get(RedirectId redirectId)
        {
            var redirectGroup = GetRedirectGroup(redirectId.Name);
            redirectGroup.TryGetValue(redirectId.Type, out var redirect);
           
            return redirect;
        }

        /// <inheritdoc />
        public IRedirectSet Reset(string? name = null)
        {
            var redirectGroup = GetRedirectGroup(name);
            foreach (var redirect in redirectGroup.Values)
            {
                redirect.Reset();
            }

            return this;
        }
        
        /// <inheritdoc />
        public IRedirectSet ResetAll()
        {
            foreach (var redirectGroup in _redirectGroups.Values)
            {
                foreach (var redirect in redirectGroup.Values)
                {
                    redirect.Reset();
                }
            }

            return this;
        }
        
        /// <inheritdoc />
        public IRedirectSet Strict(string? name = null)
        {
            var redirectGroup = GetRedirectGroup(name);
            foreach (var redirect in redirectGroup.Values)
            {
                redirect.Strict();
            }

            return this;
        }
        
        /// <inheritdoc />
        public IRedirectSet StrictAll()
        {
            foreach (var redirectGroup in _redirectGroups.Values)
            {
                foreach (var redirect in redirectGroup.Values)
                {
                    redirect.Strict();
                }
            }

            return this;
        }
        
        /// <summary>
        /// Add a <see cref="IRedirect{TTarget}"/> to this <see cref="IRedirectSet"/>. For internal use by <see cref="IRedirect{TTarget}"/> constructor.
        /// </summary>
        /// <param name="redirect">The <see cref="IRedirect{TTarget}"/> instance to add.</param>
        /// <exception cref="DiverterException">Thrown if <see cref="IRedirect{TTarget}"/> already exists in this <see cref="IRedirectSet"/></exception>
        internal void AddRedirect(IRedirect redirect)
        {
            var redirectGroup = GetRedirectGroup(redirect.RedirectId.Name);
            if (!redirectGroup.TryAdd(redirect.RedirectId.Type, redirect))
            {
                throw new DiverterException("Redirect already exists in RedirectSet");
            }
        }

        private ConcurrentDictionary<Type, IRedirect> GetRedirectGroup(string? name = null)
        {
            return _redirectGroups.GetOrAdd(name ?? string.Empty, _ => new ConcurrentDictionary<Type, IRedirect>());
        }
    }
}