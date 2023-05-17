using System;
using System.Threading.Tasks;
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
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, Task<TReturn>> redirectUpdater, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return redirectUpdater.ViaRedirect(null, optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, ValueTask<TReturn>> redirectUpdater, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return redirectUpdater.ViaRedirect(null, optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, TReturn> redirectUpdater, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return ViaRedirectInternal<TTarget, TReturn>(redirectUpdater, name, redirect => new ViaRedirectCallHandler<TReturn>(redirect), optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, Task<TReturn>> redirectUpdater, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return ViaRedirectInternal<TTarget, TReturn>(redirectUpdater, name, redirect => new ViaRedirectTaskCallHandler<TReturn>(redirect), optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IFuncRedirectUpdater<TTarget, ValueTask<TReturn>> redirectUpdater, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return ViaRedirectInternal<TTarget, TReturn>(redirectUpdater, name, redirect => new ViaRedirectValueTaskCallHandler<TReturn>(redirect), optionsAction);
        }

        private static IRedirect<TReturn> ViaRedirectInternal<TTarget, TReturn>(object redirectUpdater, string? name, Func<IRedirect<TReturn>, ICallHandler> callHandlerFactory, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            if (redirectUpdater is not RedirectUpdater<TTarget> concreteUpdater)
            {
                throw new ArgumentException("This extension only supports an internal concrete implementation of this interface");
            }
            
            var redirect = concreteUpdater.Redirect.RedirectSet.GetOrCreate<TReturn>(name);
            var callHandler = callHandlerFactory.Invoke(redirect);
            concreteUpdater.Via(callHandler, optionsAction);

            return redirect;
        }
    }
}