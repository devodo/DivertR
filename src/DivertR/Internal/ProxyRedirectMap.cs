using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ProxyRedirectMap
    {
        private readonly ConditionalWeakTable<object, IRedirect> _redirectMap = new();

        public void AddRedirect(IRedirect redirect, object proxy)
        {
            _redirectMap.Add(proxy, redirect);
        }

        public IRedirect<TTarget> GetRedirect<TTarget>(TTarget proxy) where TTarget : class
        {
            if (!_redirectMap.TryGetValue(proxy, out var redirect))
            {
                throw new DiverterException("Redirect not found");
            }

            if (redirect is not IRedirect<TTarget> redirectOf)
            {
                throw new DiverterException($"Redirect target type: {redirect.RedirectId.Type} does not match proxy type: {typeof(TTarget)}");
            }

            return redirectOf;
        }
    }
}