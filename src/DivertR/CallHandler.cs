using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class CallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class?
    {
        private readonly Func<IRedirectCall<TTarget>, object?> _redirectDelegate;

        public CallHandler(Func<IRedirectCall<TTarget>, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
    
    public class CallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, object?> _redirectDelegate;

        public CallHandler(Func<IRedirectCall, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate ?? throw new ArgumentNullException(nameof(redirectDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
}