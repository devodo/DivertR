using System;
using System.Collections.Concurrent;
using DivertR.Internal;

namespace DivertR.Default.Internal
{
    internal class DummyFactory : IDummyFactory
    {
        private readonly ConcurrentDictionary<Type, object> _proxyCache = new ConcurrentDictionary<Type, object>();
        private readonly RedirectRepository _redirectRepository = new RedirectRepository();
        
        public DummyFactory(IDefaultValueFactory defaultValueFactory)
        {
            var callHandler = new DefaultRootCallHandler(defaultValueFactory);
            var redirect = new Redirect(callHandler);
            _redirectRepository.InsertRedirect(redirect);
        }

        public TTarget Create<TTarget>(DiverterSettings settings) where TTarget : class
        {
            var proxy = _proxyCache.GetOrAdd(typeof(TTarget), type =>
            {
                var via = CreateRootVia<TTarget>(settings);

                return via.Proxy(null);
            }) as IVia<TTarget>;
            
            return (proxy as TTarget)!;
        }

        public void Redirect<TReturn>(Func<IRedirectCall, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            object? InnerDelegate(IRedirectCall call) => redirectDelegate.Invoke(call);
            var callHandler = new FuncRedirectCallHandler(InnerDelegate);
            var callConstraint = new ReturnTypeCallConstraint<TReturn>();
            var redirect = new Redirect(callHandler, callConstraint);
            _redirectRepository.InsertRedirect(redirect);
        }

        private IVia<TTarget> CreateRootVia<TTarget>(DiverterSettings settings) where TTarget : class
        {
            var via = new Via<TTarget>(settings);
            via.InsertRedirects(_redirectRepository.RedirectPlan.Redirects);

            return via;
        }
    }
}