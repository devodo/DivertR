using System;

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
            var via = viaBuilder.Via.ViaSet.Via<TReturn>(name);
            ICallHandler<TTarget> callHandler = new CallHandler<TTarget>(call => via.Proxy((TReturn?) call.Relay.CallNext()));
            var redirect = viaBuilder.RedirectBuilder.Build(callHandler, optionsAction);
            viaBuilder.Via.RedirectRepository.InsertRedirect(redirect);

            return via;
        }
    }
}