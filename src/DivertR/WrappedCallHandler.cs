using System.Runtime.CompilerServices;

namespace DivertR
{
    internal class WrappedCallHandler<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly ICallHandler _innerHandler;

        public WrappedCallHandler(ICallHandler innerHandler)
        {
            _innerHandler = innerHandler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _innerHandler.Handle(call);
        }
    }
}