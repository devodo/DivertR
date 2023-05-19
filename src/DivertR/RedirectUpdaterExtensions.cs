using System;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR
{
    public static class RedirectUpdaterExtensions
    {
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IRedirectToFuncBuilder<TTarget, TReturn> redirectBuilder, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return redirectBuilder.ViaRedirect(null, optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IRedirectToFuncBuilder<TTarget, Task<TReturn>> redirectBuilder, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return redirectBuilder.ViaRedirect(null, optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IRedirectToFuncBuilder<TTarget, ValueTask<TReturn>> redirectBuilder, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return redirectBuilder.ViaRedirect(null, optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IRedirectToFuncBuilder<TTarget, TReturn> redirectBuilder, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return ViaRedirectInternal<TTarget, TReturn>(redirectBuilder, name, redirect => new ViaRedirectCallHandler<TReturn>(redirect), optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IRedirectToFuncBuilder<TTarget, Task<TReturn>> redirectBuilder, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return ViaRedirectInternal<TTarget, TReturn>(redirectBuilder, name, redirect => new ViaRedirectTaskCallHandler<TReturn>(redirect), optionsAction);
        }
        
        public static IRedirect<TReturn> ViaRedirect<TTarget, TReturn>(this IRedirectToFuncBuilder<TTarget, ValueTask<TReturn>> redirectBuilder, string? name, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            return ViaRedirectInternal<TTarget, TReturn>(redirectBuilder, name, redirect => new ViaRedirectValueTaskCallHandler<TReturn>(redirect), optionsAction);
        }

        private static IRedirect<TReturn> ViaRedirectInternal<TTarget, TReturn>(object redirectUpdater, string? name, Func<IRedirect<TReturn>, ICallHandler> callHandlerFactory, Action<IViaOptionsBuilder>? optionsAction = null)
            where TTarget : class?
            where TReturn : class?
        {
            if (redirectUpdater is not RedirectToBuilder<TTarget> concreteUpdater)
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