using System;
using System.Runtime.CompilerServices;
using DivertR.Internal;

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
            var redirect = redirectUpdater.Redirect.RedirectSet.Redirect<TReturn>(name);
            
            TReturn ViaDelegate(IFuncRedirectCall<TTarget, TReturn> call)
            {
                var callReturn = call.CallNext();

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

            var via = redirectUpdater.ViaBuilder.Build(ViaDelegate);
            var options = ViaOptionsBuilder.Create(optionsAction);
            redirectUpdater.Redirect.RedirectRepository.InsertVia(via, options);

            return redirect;
        }
    }
}