using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ViaRedirectCallHandler<TReturn> : ICallHandler where TReturn : class?
    {
        private readonly IRedirect<TReturn> _redirect;

        public ViaRedirectCallHandler(IRedirect<TReturn> redirect)
        {
            _redirect = redirect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            var callReturn = call.CallNext();

            if (callReturn == null)
            {
                return null;
            }

            if (callReturn is not TReturn typedReturn)
            {
                throw new InvalidOperationException($"Unexpected return type encountered: {callReturn.GetType()}");
            }
            
            return _redirect.Proxy(typedReturn);
        }
    }
}