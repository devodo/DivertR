using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public static class RedirectUpdaterExtensions
    {
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, TReturn> redirectUpdater, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return redirectUpdater.ViaRedirect(null, optionsAction);
        }

        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, TReturn> redirectUpdater, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            var proxyCache = new ConditionalWeakTable<TReturn, TReturn>();
            var redirect = redirectUpdater.Redirect.RedirectSet.GetOrCreate<TReturn>(name);
            
            TReturn ViaDelegate(IFuncRedirectCall<TTarget, TReturn> call)
            {
                var callReturn = call.CallNext()!;

                if (callReturn == null!)
                {
                    return null!;
                }

                return proxyCache.GetValue(callReturn, x =>
                {
                    var proxy = redirect.Proxy(x);

                    return proxy;
                });
            }

            redirectUpdater.Via(ViaDelegate, optionsAction);
            
            return redirect;
        }
    }
}