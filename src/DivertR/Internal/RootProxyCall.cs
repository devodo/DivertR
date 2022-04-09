using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RootProxyCall : IProxyCall
    {
        private readonly IRelay _relay;

        public RootProxyCall(IRelay relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo callInfo)
        {
            return _relay.CallRoot(callInfo.Method, callInfo.Arguments);
        }
    }
}