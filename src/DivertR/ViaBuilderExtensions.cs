using System;
using System.Collections.Concurrent;

namespace DivertR
{
    public static class ViaBuilderExtensions
    {
        public static IVia<TReturn> RedirectVia<TTarget, TReturn>(this IFuncViaBuilder<TTarget, TReturn> viaBuilder, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TTarget : class
            where TReturn : class
        {
            return viaBuilder.RedirectVia(null, optionsAction);
        }

        public static IVia<TReturn> RedirectVia<TTarget, TReturn>(this IFuncViaBuilder<TTarget, TReturn> viaBuilder, string? name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TTarget : class
            where TReturn : class
        {
            var proxyCache = new ConcurrentDictionary<object, TReturn>();
            var via = viaBuilder.Via.ViaSet.Via<TReturn>(name);

            TReturn RedirectDelegate(IFuncRedirectCall<TTarget, TReturn> call)
            {
                return proxyCache.GetOrAdd(call.CallNext(), callReturn => via.Proxy(callReturn));
            }

            var redirect = viaBuilder.RedirectBuilder.Build(RedirectDelegate, optionsAction);
            viaBuilder.Via.RedirectRepository.InsertRedirect(redirect);

            return via;
        }
    }
}