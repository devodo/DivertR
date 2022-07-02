using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class CallHandlerWrapper<TTarget> : ICallHandler<TTarget> where TTarget : class
    {
        private readonly ICallHandler _callHandler;

        public CallHandlerWrapper(ICallHandler callHandler)
        {
            _callHandler = callHandler;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Handle(IRedirectCall<TTarget> call)
        {
            return _callHandler.Handle(call);
        }
    }
}