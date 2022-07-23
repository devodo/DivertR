using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Func<IRedirectCall<TTarget>, object?> _redirectDelegate;

        public RedirectCallHandler(Func<IRedirectCall<TTarget>, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
    
    internal class RedirectArgsCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly Func<IRedirectCall<TTarget>, CallArguments, object?> _redirectDelegate;

        public RedirectArgsCallHandler(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _redirectDelegate.Invoke(call, call.Args);
        }
    }
    
    internal class RedirectCallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, object?> _redirectDelegate;

        public RedirectCallHandler(Func<IRedirectCall, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _redirectDelegate.Invoke(call);
        }
    }
    
    internal class RedirectArgsCallHandler : ICallHandler
    {
        private readonly Func<IRedirectCall, CallArguments, object?> _redirectDelegate;

        public RedirectArgsCallHandler(Func<IRedirectCall, CallArguments, object?> redirectDelegate)
        {
            _redirectDelegate = redirectDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _redirectDelegate.Invoke(call, call.Args);
        }
    }
}