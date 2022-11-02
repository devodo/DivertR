using System.Diagnostics.CodeAnalysis;

namespace DivertR.Internal
{
    internal class ViaProxyDecorator : IViaProxyDecorator
    {
        private readonly IProxyViaMap _proxyViaMap;

        public ViaProxyDecorator(IProxyViaMap proxyViaMap)
        {
            _proxyViaMap = proxyViaMap;
        }
        
        [return: NotNull]
        public TTarget Decorate<TTarget>(IVia via, [DisallowNull] TTarget proxy) where TTarget : class?
        {
            _proxyViaMap.AddVia(via, proxy);
            
            return proxy;
        }
    }
}