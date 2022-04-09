using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class NextProxyCall : IProxyCall
    {
        private readonly IRelay _relay;

        public NextProxyCall(IRelay relay)
        {
            _relay = relay;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Call(CallInfo callInfo)
        {
            return _relay.CallNext(callInfo.Method, callInfo.Arguments);
        }
    }
}