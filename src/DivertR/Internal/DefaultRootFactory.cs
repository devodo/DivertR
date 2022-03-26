using System;

namespace DivertR.Internal
{
    internal class DefaultRootFactory : IDefaultRootFactory
    {
        private readonly IProxyFactory _proxyFactory;

        public DefaultRootFactory(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }
        
        public TTarget Create<TTarget>() where TTarget : class
        {
            var via = new Via<TTarget>();
            var callHandler = new DefaultRootCallHandler();
            var redirect = new Redirect(callHandler, new TrueCallConstraint());
            via.InsertRedirect(redirect);

            return via.Proxy(null);
        }
    }
}