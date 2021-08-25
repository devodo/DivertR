using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class OriginalProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;

        public OriginalProxyCall(IRelay<TTarget> relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relay.CallOriginal(callInfo.Method, callInfo.Arguments);
        }
    }
}