using System;
using System.Collections.Concurrent;

namespace DivertR.Default
{
    public class DefaultRootFactory : IDefaultRootFactory
    {
        private readonly DiverterSettings _settings;
        private readonly ConcurrentDictionary<Type, IVia> _viaCache = new ConcurrentDictionary<Type, IVia>();
        private readonly Redirect _defaultRedirect;
        
        public DefaultRootFactory(IDefaultValueFactory defaultValueFactory, DiverterSettings? settings = null)
        {
            _settings = settings ?? DiverterSettings.Global;
            var defaultHandler = new DefaultRootCallHandler(defaultValueFactory);
            _defaultRedirect = new Redirect(defaultHandler, TrueCallConstraint.Instance);
        }

        protected virtual IVia<TTarget> CreateRootVia<TTarget>() where TTarget : class
        {
            var via = new Via<TTarget>(_settings);
            via.InsertRedirect(_defaultRedirect);

            return via;
        }
        
        public TTarget CreateRoot<TTarget>() where TTarget : class
        {
            var via = _viaCache.GetOrAdd(typeof(TTarget), type => CreateRootVia<TTarget>()) as IVia<TTarget>;
            
            return via!.Proxy(null);
        }
    }
}