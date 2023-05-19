using System;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class FuncCallHandler<TReturn> : ICallHandler
    {
        private readonly Func<TReturn?> _handlerDelegate;

        public FuncCallHandler(Func<TReturn?> handlerDelegate)
        {
            _handlerDelegate = handlerDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall call)
        {
            return _handlerDelegate.Invoke();
        }
    }
}