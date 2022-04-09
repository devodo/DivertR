using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class DelegateCallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, object?> _redirectDelegate;

        public DelegateCallHandler(Func<IRedirectCall, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(IRedirectCall call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
    
    internal class DelegateCallHandler<TTarget> : CallHandler<TTarget> where TTarget : class
    {
        private readonly Func<IRedirectCall<TTarget>, object?> _redirectDelegate;

        public DelegateCallHandler(Func<IRedirectCall<TTarget>, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object? Call(IRedirectCall<TTarget> call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
}