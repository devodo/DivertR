using System;
using System.Collections.Concurrent;

namespace DivertR.Default.Internal
{
    internal class DummyFactory : IDummyFactory
    {
        private readonly ConcurrentDictionary<Type, object> _proxyCache = new ConcurrentDictionary<Type, object>();
        private readonly DummyVia _via;
        
        public DummyFactory(IDefaultValueFactory defaultValueFactory, DiverterSettings diverterSettings)
        {
            var callHandler = new DefaultRootCallHandler(defaultValueFactory);
            var redirect = new Redirect(callHandler);

            _via = new DummyVia(diverterSettings);
            _via.InsertRedirect(redirect);
        }

        public TTarget Create<TTarget>() where TTarget : class
        {
            var proxy = _proxyCache.GetOrAdd(typeof(TTarget), type => _via.Proxy<TTarget>());
            
            return (proxy as TTarget)!;
        }
    }
}