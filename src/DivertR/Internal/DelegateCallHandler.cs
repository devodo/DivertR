using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class DelegateCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Func<CallInfo<TTarget>, object?> _redirectDelegate;

        public DelegateCallHandler(Func<CallInfo<TTarget>, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _redirectDelegate.Invoke(callInfo);
        }
    }
}