using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Action<IRedirectCall<TTarget>> _redirectDelegate;

        public RedirectCallHandler(Action<IRedirectCall<TTarget>> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            _redirectDelegate.Invoke(call);

            return default;
        }
    }
}