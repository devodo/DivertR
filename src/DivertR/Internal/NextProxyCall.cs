using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class NextProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;

        public NextProxyCall(IRelay<TTarget> relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relay.CallNext(callInfo.Method, callInfo.Arguments);
        }
    }
}