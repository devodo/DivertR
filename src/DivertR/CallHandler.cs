using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public class CallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class?
    {
        private readonly Func<IRedirectCall<TTarget>, object?> _handlerDelegate;

        public CallHandler(Func<IRedirectCall<TTarget>, object?> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate ?? throw new ArgumentNullException(nameof(handlerDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _handlerDelegate.Invoke(call);
        }
    }
    
    public class CallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, object?> _handlerDelegate;

        public CallHandler(Func<IRedirectCall, object?> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate ?? throw new ArgumentNullException(nameof(handlerDelegate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _handlerDelegate.Invoke(call);
        }
    }
}