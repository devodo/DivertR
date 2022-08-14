using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public static class ViaBuilderExtensions
    {
        public static IVia<TReturn> RedirectVia<TTarget, TReturn>(this IFuncViaBuilder<TTarget, TReturn> viaBuilder, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return viaBuilder.RedirectVia(null, optionsAction);
        }

        public static IVia<TReturn> RedirectVia<TTarget, TReturn>(this IFuncViaBuilder<TTarget, TReturn> viaBuilder, string? name, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            var proxyCache = new ConditionalWeakTable<TReturn, TReturn>();
            var via = viaBuilder.Via.ViaSet.Via<TReturn>(name);
            
            TReturn RedirectDelegate(IFuncRedirectCall<TTarget, TReturn> call)
            {
                var callReturn = call.CallNext();

                if (callReturn == null!)
                {
                    return null!;
                }

                return proxyCache.GetValue(callReturn, x =>
                {
                    var proxy = via.Proxy(x);

                    return proxy;
                });
            }

            var redirect = viaBuilder.RedirectBuilder.Build(RedirectDelegate, optionsAction);
            viaBuilder.Via.RedirectRepository.InsertRedirect(redirect);

            return via;
        }
    }
}