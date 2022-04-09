using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectCallHandler : ICallHandler
    {
        private readonly Action<IRedirectCall> _redirectDelegate;

        public RedirectCallHandler(Action<IRedirectCall> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(IRedirectCall call)
        {
            _redirectDelegate.Invoke(call);

            return default;
        }
    }
}