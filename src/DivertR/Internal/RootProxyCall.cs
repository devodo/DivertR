using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RootProxyCall<TTarget> : IProxyCall<TTarget> where TTarget : class
    {
        private readonly IRelay<TTarget> _relay;

        public RootProxyCall(IRelay<TTarget> relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo<TTarget> callInfo)
        {
            return _relay.CallRoot(callInfo.Method, callInfo.Arguments);
        }
    }
}