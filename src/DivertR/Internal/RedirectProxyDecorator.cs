using System.Diagnostics.CodeAnalysis;

namespace DivertR.Internal
{
    internal class RedirectProxyDecorator : IRedirectProxyDecorator
    {
        private readonly IProxyRedirectMap _proxyRedirectMap;

        public RedirectProxyDecorator(IProxyRedirectMap proxyRedirectMap)
        {
            _proxyRedirectMap = proxyRedirectMap;
        }
        
        [return: NotNull]
        public TTarget Decorate<TTarget>(IRedirect redirect, [DisallowNull] TTarget proxy) where TTarget : class?
        {
            _proxyRedirectMap.AddRedirect(redirect, proxy);
            
            return proxy;
        }
    }
}