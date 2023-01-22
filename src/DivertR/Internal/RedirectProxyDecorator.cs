using System.Diagnostics.CodeAnalysis;

namespace DivertR.Internal
{
    internal class RedirectProxyDecorator : IRedirectProxyDecorator
    {
        [return: NotNull]
        public TTarget Decorate<TTarget>(IRedirect redirect, [DisallowNull] TTarget proxy) where TTarget : class?
        {
            return proxy;
        }
    }
}